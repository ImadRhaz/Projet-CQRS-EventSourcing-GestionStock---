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

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 2. Services
// 2.1 Add controllers
builder.Services.AddControllers();

// 2.2 Add endpoints API explorer
builder.Services.AddEndpointsApiExplorer();

// 2.3 Add Swagger
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
            new string[] {}
        }
    });
});

// 2.4 Database Configuration
builder.Services.AddDbContext<QueryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QueryDbConnection")));

builder.Services.AddDbContext<EventStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventStoreConnection")));

// 2.5 RabbitMQ Configuration
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMqConfiguration"));

// 2.6 Register Dependencies
builder.Services.AddSingleton<RabbitMqCommandBus>();
builder.Services.AddSingleton<RabbitMqEventBus>();

// Handlers pour User
builder.Services.AddScoped<ICommandHandler<RegisterUserCommand>, RegisterUserCommandHandler>();
builder.Services.AddScoped<IEventHandler<UserCreatedEvent>, GestionFM1.Read.EventHandlers.UserCreatedEventHandler>(); // <-- Référence mise à jour

// Repositories
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<UserWriteRepository>();

// 2.7 Configure Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<QueryDbContext>()
    .AddDefaultTokenProviders();

// 2.8 Add Hosted Services (Consumers)
builder.Services.AddHostedService<RegisterUserCommandConsumer>();

// 2.9 JWT Configuration
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

// 2.10 Add Logging
builder.Services.AddLogging();

var app = builder.Build();

// 3. Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 4. Create roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    string[] roleNames = { "Admin", "Gestionnaire", "Utilisateur" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (roleResult.Succeeded)
            {
                logger.LogInformation($"Created role: {roleName}");
            }
            else
            {
                logger.LogError($"Error creating role: {roleName}");
                foreach (var error in roleResult.Errors)
                {
                    logger.LogError(error.Description);
                }
            }
        }
    }
}

app.Run();