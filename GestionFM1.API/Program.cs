using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.DTOs;
using GestionFM1.Infrastructure.Configuration;
using GestionFM1.Infrastructure.Messaging;
using GestionFM1.Read.QueryDataStore;
using GestionFM1.Read.Repositories;
using GestionFM1.Write.CommandHandlers;
using GestionFM1.Read.EventHandlers;
using GestionFM1.Write.EventStore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GestionFM1.Utilities;
using Microsoft.Extensions.Logging;
using GestionFM1.Core.Events;
using GestionFM1.Write.Repositories;
using GestionFM1.Write.CommandConsumer;
using GestionFM1.Write.Commands;
using Newtonsoft.Json;
using GestionFM1.Read.QueryHandlers;
using System.Collections.Generic;
using GestionFM1.Read.Queries;
using GestionFM1.Controllers;
var builder = WebApplication.CreateBuilder(args);

// 1. Chargement de la configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 2. Ajout des services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Replace with your frontend's URL
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Important for cookies and authorization headers
        });
});

// 3. Configuration Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GestionFM1 API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// 4. Configuration de la base de données
builder.Services.AddDbContext<QueryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QueryDbConnection")));

builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventStoreConnection")));

// 5. Configuration RabbitMQ
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMqConfiguration"));
builder.Services.AddSingleton<RabbitMqCommandBus>();
builder.Services.AddSingleton<RabbitMqEventBus>();

// 6. Injection des dépendances
// 6.1 Handlers pour User
builder.Services.AddScoped<ICommandHandler<RegisterUserCommand>, RegisterUserCommandHandler>();
builder.Services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();

// 6.2 Handlers pour FM1
builder.Services.AddScoped<ICommandHandler<AddFM1Command>, AddFM1CommandHandler>();
builder.Services.AddScoped<IEventHandler<FM1CreatedEvent>, FM1CreatedEventHandler>();

// 6.3 Handlers pour Composent
builder.Services.AddScoped<ICommandHandler<AddComposentCommand>, AddComposentCommandHandler>();
builder.Services.AddScoped<IEventHandler<ComposentCreatedEvent>, ComposentCreatedEventHandler>();

// 6.4 Handlers pour Commande (Ajout des services pour Commande)
builder.Services.AddScoped<ICommandHandler<CommandeAddCommand>, CommandeAddCommandHandler>();
builder.Services.AddScoped<IEventHandler<CommandeCreatedEvent>, CommandeCreatedEventHandler>(); // Garder

// 6.5 Handlers pour FM1History
builder.Services.AddScoped<ICommandHandler<AddFM1HistoryCommand>, AddFM1HistoryCommandHandler>();
builder.Services.AddScoped<IEventHandler<FM1HistoryCreatedEvent>, FM1HistoryCreatedEventHandler>();

// 6.6 Repositories
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<UserWriteRepository>();
builder.Services.AddScoped<FM1WriteRepository>();
builder.Services.AddScoped<ComposentWriteRepository>();
builder.Services.AddScoped<CommandeWriteRepository>(); // Ajout de CommandeWriteRepository
builder.Services.AddScoped<FM1HistoryWriteRepository>();

// 7. Configuration Identity

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<QueryDbContext>()
    .AddDefaultTokenProviders(); // Ajout pour 2FA

// Configuration des options Identity pour 2FA
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Tokens.AuthenticatorTokenProvider = "Authenticator";
});// 7. Configuration Identity

// 8. Ajout des services hébergés (Consumers)
builder.Services.AddHostedService<RegisterUserCommandConsumer>();
builder.Services.AddHostedService<AddFM1CommandConsumer>();
builder.Services.AddHostedService<AddComposentCommandConsumer>();
builder.Services.AddHostedService<CommandeAddCommandConsumer>(); // Ajout de CommandeAddCommandConsumer
builder.Services.AddHostedService<AddFM1HistoryCommandConsumer>();

// 9. Configuration JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

// 10. Ajout du logging
builder.Services.AddLogging();

// 11. Ajout des Query Handlers
builder.Services.AddScoped<LoginQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetUserRolesQuery, IList<string>>, GetUserRolesQueryHandler>();

// 11.1 Ajout des Query Handlers pour FM1
builder.Services.AddScoped<IQueryHandler<GetAllFM1Query, IEnumerable<FM1>>, GetAllFM1QueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetFM1ByIdQuery, FM1>, GetFM1ByIdQueryHandler>();

// 11.2 Ajout des Query Handlers pour Composent
builder.Services.AddScoped<IQueryHandler<GetAllComposentsQuery, IEnumerable<Composent>>, GetAllComposentsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetComposentByIdQuery, Composent>, GetComposentByIdQueryHandler>();

// 11.3 Ajout des Query Handlers pour Commande
builder.Services.AddScoped<IQueryHandler<GetAllCommandesQuery, IEnumerable<Commande>>, GetAllCommandesQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetCommandeByIdQuery, Commande>, GetCommandeByIdQueryHandler>();

// 11.4 Ajout des Query Handlers pour FM1History
builder.Services.AddScoped<IQueryHandler<GetAllFM1HistoriesQuery, IEnumerable<FM1History>>, GetAllFM1HistoriesQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetFM1HistoryByIdQuery, FM1History>, GetFM1HistoryByIdQueryHandler>();  //Add this 

// 11.5 Registration for  GetComposentsByFM1IdQuery

builder.Services.AddScoped<IQueryHandler<GetComposentsByFM1IdQuery, IEnumerable<Composent>>, GetComposentsByFM1IdQueryHandler>();
//11.5 Ajout des Repositories Read
builder.Services.AddScoped<IFM1ReadRepository, FM1ReadRepository>();
builder.Services.AddScoped<IComposentReadRepository, ComposentReadRepository>();
builder.Services.AddScoped<ICommandeReadRepository, CommandeReadRepository>();
builder.Services.AddScoped<IFM1HistoryReadRepository, FM1HistoryReadRepository>();

builder.Services.AddScoped<IQueryHandler<GetFM1HistoryByFM1IdQuery, FM1History>, GetFM1HistoryByFM1IdQueryHandler>();
// ... dans la méthode ConfigureServices

builder.Services.AddScoped<IQueryHandler<GetFM1HistoryByFM1IdQuery, FM1History>, GetFM1HistoryByFM1IdQueryHandler>();

// Ajouter le contrôleur pour ExcelFm1
builder.Services.AddScoped<ExcelFm1Controller>();
var app = builder.Build();

// 12. Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowSpecificOrigin");


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 13. Création des rôles (Admin, Gestionnaire, Utilisateur)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    string[] roleNames = { "Expert", "Magasinier" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (roleResult.Succeeded)
            {
                logger.LogInformation($"[Roles] Création du rôle : {roleName}");
            }
            else
            {
                logger.LogError($"[Roles] Erreur lors de la création du rôle : {roleName}");
                foreach (var error in roleResult.Errors)
                {
                    logger.LogError(error.Description);
                }
            }
        }
    }
}

// 14. Abonnement aux événements RabbitMQ avec gestion des erreurs
var eventBus = app.Services.GetRequiredService<RabbitMqEventBus>();
eventBus.Subscribe<FM1CreatedEvent>("gestionfm1.fm1.events", async (message) =>
{
    using var scope = app.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<FM1CreatedEvent>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var @event = JsonConvert.DeserializeObject<FM1CreatedEvent>(message);
        if (@event == null)
        {
            logger.LogError("[RabbitMQ] Événement FM1CreatedEvent NULL reçu, impossible de le traiter !");
            return;
        }

        logger.LogInformation($"[RabbitMQ] Message FM1CreatedEvent reçu : {@event.FM1Id}");

        await handler.Handle(@event);

        logger.LogInformation($"[RabbitMQ] Message FM1CreatedEvent traité avec succès : {@event.FM1Id}");
    }
    catch (Exception ex)
    {
        logger.LogError($"[RabbitMQ] Erreur lors du traitement de FM1CreatedEvent : {ex.Message}");
    }
});

// Abonnement aux événements RabbitMQ pour ComposentCreatedEvent
eventBus.Subscribe<ComposentCreatedEvent>("gestionfm1.composent.events", async (message) =>
{
    using var scope = app.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<ComposentCreatedEvent>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var @event = JsonConvert.DeserializeObject<ComposentCreatedEvent>(message);
        if (@event == null)
        {
            logger.LogError("[RabbitMQ] Événement ComposentCreatedEvent NULL reçu, impossible de le traiter !");
            return;
        }

        logger.LogInformation($"[RabbitMQ] Message ComposentCreatedEvent reçu : {@event.ComposentId}");

        await handler.Handle(@event);

        logger.LogInformation($"[RabbitMQ] Message ComposentCreatedEvent traité avec succès : {@event.ComposentId}");
    }
    catch (Exception ex)
    {
        logger.LogError($"[RabbitMQ] Erreur lors du traitement de ComposentCreatedEvent : {ex.Message}");
    }
});

// 15. Abonnement aux événements RabbitMQ pour CommandeCreatedEvent (Ajout de l'abonnement pour Commande)
eventBus.Subscribe<CommandeCreatedEvent>("gestionfm1.commande.events", async (message) =>
{
    using var scope = app.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<CommandeCreatedEvent>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var @event = JsonConvert.DeserializeObject<CommandeCreatedEvent>(message);
        if (@event == null)
        {
            logger.LogError("[RabbitMQ] Événement CommandeCreatedEvent NULL reçu, impossible de le traiter !");
            return;
        }

        logger.LogInformation($"[RabbitMQ] Message CommandeCreatedEvent reçu : {@event.CommandeId}");

        await handler.Handle(@event);

        logger.LogInformation($"[RabbitMQ] Message CommandeCreatedEvent traité avec succès : {@event.CommandeId}");
    }
    catch (Exception ex)
    {
        logger.LogError($"[RabbitMQ] Erreur lors du traitement de CommandeCreatedEvent : {ex.Message}");
    }
});

// 16. Abonnement aux événements RabbitMQ pour FM1HistoryCreatedEvent
eventBus.Subscribe<FM1HistoryCreatedEvent>("gestionfm1.fm1history.events", async (message) =>
{
    using var scope = app.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<FM1HistoryCreatedEvent>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var @event = JsonConvert.DeserializeObject<FM1HistoryCreatedEvent>(message);
        if (@event == null)
        {
            logger.LogError("[RabbitMQ] Événement FM1HistoryCreatedEvent NULL reçu, impossible de le traiter !");
            return;
        }

        logger.LogInformation($"[RabbitMQ] Message FM1HistoryCreatedEvent reçu : {@event.FM1HistoryId}");

        await handler.Handle(@event);

        logger.LogInformation($"[RabbitMQ] Message FM1HistoryCreatedEvent traité avec succès : {@event.FM1HistoryId}");
    }
    catch (Exception ex)
    {
        logger.LogError($"[RabbitMQ] Erreur lors du traitement de FM1HistoryCreatedEvent : {ex.Message}");
    }
});

app.Run();