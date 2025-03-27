using Microsoft.AspNetCore.Mvc;
using GestionFM1.Write.Commands;
using GestionFM1.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GestionFM1.DTOs;
using System;
using GestionFM1.Read.QueryDataStore;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Models;
using Microsoft.AspNetCore.Authorization;
using GestionFM1.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace GestionFM1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommandController : ControllerBase
    {
        private readonly RabbitMqCommandBus _commandBus;
        private readonly ILogger<CommandController> _logger;
        private readonly QueryDbContext _queryDbContext;
        private readonly RabbitMqConfiguration _rabbitMqConfig;

        public CommandController(
            RabbitMqCommandBus commandBus,
            ILogger<CommandController> logger,
            QueryDbContext queryDbContext,
            IOptions<RabbitMqConfiguration> rabbitMqConfig)
        {
            _commandBus = commandBus;
            _logger = logger;
            _queryDbContext = queryDbContext;
            _rabbitMqConfig = rabbitMqConfig.Value;
        }

        [AllowAnonymous] 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return BadRequest(ModelState);
            }

            var command = new RegisterUserCommand
            {
                Nom = registerDto.Nom,
                Prenom = registerDto.Prenom,
                Email = registerDto.Email,
                Password = registerDto.Password,
                UserType = registerDto.UserType
            };

            try
            {
                _logger.LogInformation($"Sending RegisterUserCommand for email: {registerDto.Email}");
                await _commandBus.SendCommandAsync(command, "gestionfm1.commands");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while registering user with email: {registerDto.Email}");
                return BadRequest(ex.Message);
            }
        }
       [AllowAnonymous]
         [HttpPost("add-fm1")]
        public async Task<IActionResult> AddFM1([FromBody] AddFM1DTO addFM1Dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return BadRequest(ModelState);
            }

            var command = new AddFM1Command
            {
                CodeSite = addFM1Dto.CodeSite,
                DeviceType = addFM1Dto.DeviceType,
                PsSn = addFM1Dto.PsSn,
                DateEntre = addFM1Dto.DateEntre,
                ExpirationVerification = addFM1Dto.ExpirationVerification,
                Status = addFM1Dto.Status,
                ExpertId = addFM1Dto.ExpertId
            };

            try
            {
                _logger.LogInformation($"Sending AddFM1Command for CodeSite: {addFM1Dto.CodeSite}");
                await _commandBus.SendCommandAsync(command, "gestionfm1.fm1.commands");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while adding FM1 with CodeSite: {addFM1Dto.CodeSite}");
                return BadRequest(ex.Message);
            }
        }

          [Authorize]
        [HttpPost("add-composent")]
        public async Task<IActionResult> AddComposent([FromBody] AddComposentDTO addComposentDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return BadRequest(ModelState);
            }

            var command = new AddComposentCommand
            {
                ItemBaseId = addComposentDto.ItemBaseId,
                ProductName = addComposentDto.ProductName,
                SN = addComposentDto.SN,
                TotalAvailable = addComposentDto.TotalAvailable,
                UrgentOrNot = addComposentDto.UrgentOrNot,
                OrderOrNot = addComposentDto.OrderOrNot,
                FM1Id = addComposentDto.FM1Id
            };

            try
            {
                _logger.LogInformation($"Sending AddComposentCommand for ProductName: {addComposentDto.ProductName}");
                await _commandBus.SendCommandAsync(command, "gestionfm1.composent.commands");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while adding Composent with ProductName: {addComposentDto.ProductName}");
                return BadRequest(ex.Message);
            }
        }

         [HttpPost("add-commande")]
        public async Task<IActionResult> AddCommande([FromBody] CommandeAddDTO commandeAddDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return BadRequest(ModelState);
            }

            Guid? fm1HistoryId = null;

            // Vérifier si un FM1History existe déjà pour cet FM1
            _logger.LogInformation($"Sending CommandeAddCommand for ComposentId: {commandeAddDTO.ComposentId}");

            // Creating a new scope inside action method
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                 var context = scope.ServiceProvider.GetRequiredService<QueryDbContext>();

                 var existingFM1History = await context.FM1Histories
                      .FirstOrDefaultAsync(vh => vh.FM1Id == commandeAddDTO.FM1Id);

                if (existingFM1History == null)
                {
                         _logger.LogInformation($"Creating a new FM1History pour FM1Id : {commandeAddDTO.FM1Id}");

                       // Creating un nouvelle FM1History
                       var newFM1History = new FM1History
                       {
                          Id = Guid.NewGuid(),
                          FM1Id = commandeAddDTO.FM1Id
                       };

                        context.FM1Histories.Add(newFM1History);
                        await context.SaveChangesAsync(); // Sauvegarder pour obtenir l'ID

                        fm1HistoryId = newFM1History.Id; // Récupérer l'ID du nouvel FM1History

                        // Récupérer l'FM1 correspondant
                        var fm1 = await context.FM1s.FindAsync(commandeAddDTO.FM1Id);
                        if (fm1 != null)
                        {
                            fm1.FM1HistoryId = newFM1History.Id;  // Mettre à jour FM1HistoryId
                             //context.Entry(fm1).State = EntityState.Modified; //Enlever car SaveChange va le faire
                            await context.SaveChangesAsync(); // Sauvegarder les changements sur FM1

                            _logger.LogInformation($"FM1HistoryId mis à jour dans FM1 (ID: {fm1.Id})");
                        }
                        else
                        {
                            _logger.LogWarning($"FM1 (ID: {commandeAddDTO.FM1Id}) non trouvé.");
                        }
                }
                else
                  {
                     fm1HistoryId = existingFM1History.Id; // Utiliser l'ID de l'FM1History existant
                     }
                } // the scope get diposed here so the object do not pass anymore.

                 // Creating new Command and pass fm1HistoryId
                 var command = new CommandeAddCommand
                 {
                    EtatCommande = commandeAddDTO.EtatCommande,
                    DateCmd = commandeAddDTO.DateCmd,
                    ComposentId = commandeAddDTO.ComposentId,
                    ExpertId = commandeAddDTO.ExpertId,
                    RaisonDeCommande = commandeAddDTO.RaisonDeCommande,
                    FM1Id = commandeAddDTO.FM1Id,
                    FM1HistoryId = fm1HistoryId // Passer fm1HistoryId
                };

                 try
                 {
                      _logger.LogInformation($"Sending CommandeAddCommand for ComposentId: {commandeAddDTO.ComposentId}");
                      await _commandBus.SendCommandAsync(command, "gestionfm1.commande.commands");
                     return Ok();
                }
                catch (Exception ex)
                {
                     _logger.LogError(ex, $"Error while adding Commande for ComposentId: {commandeAddDTO.ComposentId}");
                    return BadRequest(ex.Message);
                 }
            }

            [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCommandeEtat(int id, [FromBody] CommandeUpdateModel updateModel)
        {
            if (updateModel == null || string.IsNullOrWhiteSpace(updateModel.EtatCommande))
            {
                return BadRequest("EtatCommande est requis.");
            }

            var commande = await _queryDbContext.Commandes
                .Include(c => c.Expert)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
            {
                return NotFound();
            }

            commande.EtatCommande = updateModel.EtatCommande;
            await _queryDbContext.SaveChangesAsync();

            if (updateModel.EtatCommande == "Validée")
            {
                await EnvoyerNotificationCommandeValidee(commande);
            }

            return NoContent();
        }

        private async Task EnvoyerNotificationCommandeValidee(Commande commande)
{
    try
    {
        var message = new
        {
            CommandeId = commande.Id,
            ExpertId = commande.ExpertId,
            Message = $"Commande #{commande.Id} validée",
            CreatedAt = DateTime.UtcNow
        };

        await _commandBus.SendCommandAsync(
            message,
            _rabbitMqConfig.CommandeValidatedQueue);

        _logger.LogInformation($"Notification envoyée pour la commande {commande.Id}");
    }
    catch (OperationInterruptedException ex) when (ex.Message.Contains("PRECONDITION_FAILED"))
    {
        _logger.LogError(ex, "Erreur de configuration RabbitMQ. La file existe déjà avec des paramètres différents.");
        // Potentiellement relancer l'exception ou gérer différemment
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erreur inattendue lors de l'envoi de la notification");
        throw;
    }
}

            [HttpPost("add-fm1history")]
            public async Task<IActionResult> AddFM1History([FromBody] AddFM1HistoryDTO addFM1HistoryDto)
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid.");
                    return BadRequest(ModelState);
                }

                var command = new AddFM1HistoryCommand
                {
                    FM1Id = addFM1HistoryDto.FM1Id
                };

                try
                {
                    _logger.LogInformation($"Sending AddFM1HistoryCommand for FM1Id: {addFM1HistoryDto.FM1Id}");
                    await _commandBus.SendCommandAsync(command, "gestionfm1.fm1history.commands");
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while adding FM1History for FM1Id: {addFM1HistoryDto.FM1Id}");
                    return BadRequest(ex.Message);
                }
            }
    }
}