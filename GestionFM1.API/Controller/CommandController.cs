using Microsoft.AspNetCore.Mvc;
using GestionFM1.Write.Commands;
using GestionFM1.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GestionFM1.DTOs;
using System;
using GestionFM1.Read.QueryDataStore;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Models;  // <--- Ajout de cet using
using Microsoft.AspNetCore.Authorization;

namespace GestionFM1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommandController : ControllerBase
    {
        private readonly RabbitMqCommandBus _commandBus;
        private readonly ILogger<CommandController> _logger;
        private readonly QueryDbContext _queryDbContext;  // Inject your DbContext here

        public CommandController(RabbitMqCommandBus commandBus, ILogger<CommandController> logger, QueryDbContext queryDbContext) // Inject your DbContext
        {
            _commandBus = commandBus;
            _logger = logger;
            _queryDbContext = queryDbContext; // Assign it
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
                    return BadRequest("EtatCommande is required in the request body.");
                }

                _logger.LogInformation($"Attempting to update EtatCommande for Commande ID: {id} to '{updateModel.EtatCommande}'.");

                // Find the Commande in the read database
                var commande = await _queryDbContext.Commandes.FindAsync(id);

                if (commande == null)
                {
                    _logger.LogWarning($"Commande with ID: {id} not found.");
                    return NotFound();
                }

                commande.EtatCommande = updateModel.EtatCommande; // Update

                try
                {
                    await _queryDbContext.SaveChangesAsync(); // Save in read database
                    _logger.LogInformation($"Successfully updated EtatCommande for Commande ID: {id} to '{updateModel.EtatCommande}'.");
                    return NoContent();  // Returns 204 No Content
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, $"Error updating Commande ID: {id}.");
                    return StatusCode(500, "Error updating EtatCommande. See logs for details.");  // Internal Server Error
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