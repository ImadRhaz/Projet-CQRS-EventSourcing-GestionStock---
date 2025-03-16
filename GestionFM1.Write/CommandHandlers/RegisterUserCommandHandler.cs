using GestionFM1.Write.Commands;
using GestionFM1.Write.EventStore;
using GestionFM1.Core.Events;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using Microsoft.Extensions.Logging;
using GestionFM1.Infrastructure.Messaging;
using Microsoft.AspNetCore.Identity;
using System;
using GestionFM1.Core.Models;
using GestionFM1.Write.Repositories;

namespace GestionFM1.Write.CommandHandlers;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly IEventStore _eventStore;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly RabbitMqEventBus _eventBus;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserWriteRepository _userWriteRepository;

    public RegisterUserCommandHandler(
        IEventStore eventStore,
        ILogger<RegisterUserCommandHandler> logger,
        RabbitMqEventBus eventBus,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        UserWriteRepository userWriteRepository)
    {
        _eventStore = eventStore;
        _logger = logger;
        _eventBus = eventBus;
        _userManager = userManager;
        _roleManager = roleManager;
        _userWriteRepository = userWriteRepository;
    }

    public async Task Handle(RegisterUserCommand command)
    {
        try
        {
            var user = new User
            {
                Nom = command.Nom,
                Prenom = command.Prenom,
                Email = command.Email,
                UserName = command.Email
            };

            var result = await _userManager.CreateAsync(user, command.Password);

            if (result.Succeeded)
            {
                string roleName = command.UserType;

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogWarning($"Role '{roleName}' n'existe pas. L'utilisateur sera créé sans rôle.");
                    roleName = null;
                }

                if (roleName != null)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                    _logger.LogInformation($"Role '{roleName}' assigné à l'utilisateur : {command.Email}");
                }

                // Enregistrer l'utilisateur dans l'Event Store
                var userCreatedEvent = new UserCreatedEvent
                {
                    UserId = user.Id,
                    Email = command.Email,
                    Nom = command.Nom,
                    Prenom = command.Prenom,
                    UserType = command.UserType
                };

                await _eventStore.SaveEventAsync(userCreatedEvent);

                // Publier l'événement dans l'exchange dédié
                await _eventBus.PublishEventAsync(userCreatedEvent, "gestionfm1.events");

                _logger.LogInformation($"Utilisateur créé avec l'email : {command.Email}");
                _logger.LogInformation($"Événement publié via RabbitMQ pour l'utilisateur : {command.Email}");
            }
            else
            {
                _logger.LogError($"Erreur lors de la création de l'utilisateur : {command.Email}");
                foreach (var error in result.Errors)
                {
                    _logger.LogError(error.Description);
                }
                throw new Exception("Erreur lors de l'enregistrement de l'utilisateur.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erreur lors de l'enregistrement de l'utilisateur avec l'email : {command.Email}");
            throw;
        }
    }
}