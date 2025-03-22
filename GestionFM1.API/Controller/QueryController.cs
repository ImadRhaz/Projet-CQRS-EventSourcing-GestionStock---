using Microsoft.AspNetCore.Mvc;
using GestionFM1.Read.Queries;
using GestionFM1.Core.Interfaces;
using System.Threading.Tasks;
using GestionFM1.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using GestionFM1.Read.QueryHandlers;
using System;
using GestionFM1.Core.Models;
using System.Linq;
using System.Collections.Generic;



namespace GestionFM1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly LoginQueryHandler _loginQueryHandler;
        private readonly ILogger<QueryController> _logger;
        private readonly IQueryHandler<GetUserRolesQuery, IList<string>> _getUserRolesQueryHandler;
        private readonly IQueryHandler<GetAllFM1Query, IEnumerable<FM1>> _getAllFM1QueryHandler;
        private readonly IQueryHandler<GetFM1ByIdQuery, FM1> _getFM1ByIdQueryHandler;
        private readonly IQueryHandler<GetAllComposentsQuery, IEnumerable<Composent>> _getAllComposentsQueryHandler;
        private readonly IQueryHandler<GetComposentByIdQuery, Composent> _getComposentByIdQueryHandler;
        private readonly IQueryHandler<GetAllCommandesQuery, IEnumerable<Commande>> _getAllCommandesQueryHandler;
        private readonly IQueryHandler<GetCommandeByIdQuery, Commande> _getCommandeByIdQueryHandler;
        private readonly IQueryHandler<GetAllFM1HistoriesQuery, IEnumerable<FM1History>> _getAllFM1HistoriesQueryHandler;
        private readonly IQueryHandler<GetComposentsByFM1IdQuery, IEnumerable<Composent>> _getComposentsByFM1IdQueryHandler;
        private readonly IQueryHandler<GetFM1HistoryByFM1IdQuery, FM1History> _getFM1HistoryByFM1IdQueryHandler; // Added

        public QueryController(
            IQueryHandler<GetUserRolesQuery, IList<string>> getUserRolesQueryHandler,
            LoginQueryHandler loginQueryHandler,
            ILogger<QueryController> logger,
            
            IQueryHandler<GetAllFM1Query, IEnumerable<FM1>> getAllFM1QueryHandler,
            IQueryHandler<GetFM1ByIdQuery, FM1> getFM1ByIdQueryHandler,
             IQueryHandler<GetAllComposentsQuery, IEnumerable<Composent>> getAllComposentsQueryHandler,
            IQueryHandler<GetComposentByIdQuery, Composent> getComposentByIdQueryHandler,
             IQueryHandler<GetAllCommandesQuery, IEnumerable<Commande>> getAllCommandesQueryHandler,
            IQueryHandler<GetCommandeByIdQuery, Commande> getCommandeByIdQueryHandler,
             IQueryHandler<GetComposentsByFM1IdQuery, IEnumerable<Composent>> getComposentsByFM1IdQueryHandler,
            IQueryHandler<GetAllFM1HistoriesQuery, IEnumerable<FM1History>> getAllFM1HistoriesQueryHandler,
            IQueryHandler<GetFM1HistoryByFM1IdQuery, FM1History> getFM1HistoryByFM1IdQueryHandler // Added
            )
        {
            _loginQueryHandler = loginQueryHandler;
            _getUserRolesQueryHandler = getUserRolesQueryHandler;
            _logger = logger;
            _getAllFM1QueryHandler = getAllFM1QueryHandler;
            _getFM1ByIdQueryHandler = getFM1ByIdQueryHandler;
            _getAllComposentsQueryHandler = getAllComposentsQueryHandler;
            _getComposentByIdQueryHandler = getComposentByIdQueryHandler;
            _getAllCommandesQueryHandler = getAllCommandesQueryHandler;
            _getCommandeByIdQueryHandler = getCommandeByIdQueryHandler;
            _getComposentsByFM1IdQueryHandler = getComposentsByFM1IdQueryHandler;
            _getAllFM1HistoriesQueryHandler = getAllFM1HistoriesQueryHandler;
            _getFM1HistoryByFM1IdQueryHandler = getFM1HistoryByFM1IdQueryHandler; // Added
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Tentative de connexion avec un modèle invalide.");
                return BadRequest(ModelState);
            }

            var query = new LoginQuery
            {
                Email = loginDto.Email,
                Password = loginDto.Password
            };

            _logger.LogInformation($"Tentative de connexion pour l'utilisateur : {loginDto.Email}.");
            var token = await _loginQueryHandler.Handle(query);

            if (token != null)
            {
                _logger.LogInformation($"Connexion réussie pour l'utilisateur : {loginDto.Email}.");
                return Ok(new { Token = token });
            }

            _logger.LogWarning($"Échec de la connexion pour l'utilisateur : {loginDto.Email}.");
            return Unauthorized();

        }

        [HttpGet("user-roles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            _logger.LogInformation($"Récupération des rôles pour l'utilisateur avec l'ID : {userId}.");
            var result = await _getUserRolesQueryHandler.Handle(new GetUserRolesQuery { UserId = userId });

            if (result == null)
            {
                _logger.LogWarning($"Aucun rôle trouvé pour l'utilisateur avec l'ID : {userId}.");
            }
            else
            {
                _logger.LogInformation($"Rôles récupérés pour l'utilisateur avec l'ID : {userId}: {string.Join(", ", result)}");
            }

            return Ok(result);
        }

        [HttpGet("fm1s")]
        public async Task<IActionResult> GetAllFM1()
        {
            _logger.LogInformation($"Récupération de tous les FM1.");
            var result = await _getAllFM1QueryHandler.Handle(new GetAllFM1Query());

            if (result == null || !result.Any())
            {
                _logger.LogWarning($"Aucun FM1 trouvé.");
                return NotFound();
            }

            var fm1Dtos = result.Select(f => new FM1DTO
            {
                Id = f.Id,
                CodeSite = f.CodeSite,
                DeviceType = f.DeviceType,
                PsSn = f.PsSn,
                DateEntre = f.DateEntre,
                ExpirationVerification = f.ExpirationVerification,
                Status = f.Status,
                ExpertId = f.ExpertId,
                FM1HistoryId = f.FM1HistoryId
            }).ToList();

            return Ok(fm1Dtos);
        }

        [HttpGet("fm1/{id}")]
        public async Task<IActionResult> GetFM1ById(Guid id)
        {
            _logger.LogInformation($"Récupération du FM1 avec l'ID : {id}.");

            var result = await _getFM1ByIdQueryHandler.Handle(new GetFM1ByIdQuery(id));

            if (result == null)
            {
                _logger.LogWarning($"Aucun FM1 trouvé avec l'ID : {id}.");
                return NotFound();
            }

            var fm1Dto = new FM1DTO
            {
                Id = result.Id,
                CodeSite = result.CodeSite,
                DeviceType = result.DeviceType,
                PsSn = result.PsSn,
                DateEntre = result.DateEntre,
                ExpirationVerification = result.ExpirationVerification,
                Status = result.Status,
                ExpertId = result.ExpertId,
                FM1HistoryId = result.FM1HistoryId
            };

            return Ok(fm1Dto);
        }

        [HttpGet("composents")]
        public async Task<IActionResult> GetAllComposents()
        {
            _logger.LogInformation($"Récupération de tous les Composents.");
            var result = await _getAllComposentsQueryHandler.Handle(new GetAllComposentsQuery());

            if (result == null || !result.Any())
            {
                _logger.LogWarning($"Aucun Composent trouvé.");
                return NotFound();
            }

            var composentDtos = result.Select(c => new ComposentDTO
            {
                Id = c.Id,
                ItemBaseId = c.ItemBaseId,
                ProductName = c.ProductName,
                SN = c.SN,
                TotalAvailable = c.TotalAvailable,
                UrgentOrNot = c.UrgentOrNot,
                OrderOrNot = c.OrderOrNot,
                FM1Id = c.FM1Id,
                CommandeId = c.CommandeId,
                EtatCommande = c.Commande != null ? c.Commande.EtatCommande : null
            }).ToList();

            return Ok(composentDtos);
        }

        [HttpGet("composent/{id}")]
        public async Task<IActionResult> GetComposentById(Guid id)
        {
            _logger.LogInformation($"Récupération du Composent avec l'ID : {id}.");

            var result = await _getComposentByIdQueryHandler.Handle(new GetComposentByIdQuery(id));

            if (result == null)
            {
                _logger.LogWarning($"Aucun Composent trouvé avec l'ID : {id}.");
                return NotFound();
            }

            var composentDto = new ComposentDTO
            {
                Id = result.Id,
                ItemBaseId = result.ItemBaseId,
                ProductName = result.ProductName,
                SN = result.SN,
                TotalAvailable = result.TotalAvailable,
                UrgentOrNot = result.UrgentOrNot,
                OrderOrNot = result.OrderOrNot,
                FM1Id = result.FM1Id,
                CommandeId = result.CommandeId
            };

            return Ok(composentDto);
        }

        [HttpGet("composents/by-fm1/{fm1Id}")]
        public async Task<IActionResult> GetComposentsByFM1Id(Guid fm1Id)
        {
            _logger.LogInformation($"Récupération des Composents pour le FM1 avec l'ID : {fm1Id}.");

            var result = await _getComposentsByFM1IdQueryHandler.Handle(new GetComposentsByFM1IdQuery(fm1Id));

            if (result == null || !result.Any())
            {
                _logger.LogWarning($"Aucun Composent trouvé pour le FM1 avec l'ID : {fm1Id}.  Returning empty list.");
                return Ok(new List<ComposentDTO>());
            }

            var composentDtos = result.Select(c => new ComposentDTO
            {
                Id = c.Id,
                ItemBaseId = c.ItemBaseId,
                ProductName = c.ProductName,
                SN = c.SN,
                TotalAvailable = c.TotalAvailable,
                UrgentOrNot = c.UrgentOrNot,
                OrderOrNot = c.OrderOrNot,
                FM1Id = c.FM1Id,
                CommandeId = c.CommandeId,
                EtatCommande = c.Commande != null ? c.Commande.EtatCommande : null
            }).ToList();

            return Ok(composentDtos);
        }

        [HttpGet("commandes")]
        public async Task<IActionResult> GetAllCommandes()
        {
            _logger.LogInformation($"Récupération de toutes les Commandes.");
            var commandes = await _getAllCommandesQueryHandler.Handle(new GetAllCommandesQuery());

            if (commandes == null || !commandes.Any())
            {
                _logger.LogWarning($"Aucune Commande trouvée.");
                return NotFound();
            }

            var commandeDtos = commandes.Select(c => new CommandeDetailsDTO
            {
                Id = c.Id,
                EtatCommande = c.EtatCommande,
                DateCmd = c.DateCmd,
                ComposentId = c.ComposentId,
                ExpertId = c.ExpertId,
                RaisonDeCommande = c.RaisonDeCommande,
                FM1Id = c.FM1Id,
                FM1HistoryId = c.FM1HistoryId,
                ExpertNom = c.Expert?.Nom ?? string.Empty,
                ExpertPrenom = c.Expert?.Prenom ?? string.Empty,
                ComposentProductName = c.Composent?.ProductName ?? string.Empty,
                ComposentSN = c.Composent?.SN,
                ComposentUrgentOrNot = c.Composent?.UrgentOrNot ?? string.Empty,
                ComposentOrderOrNot = c.Composent?.OrderOrNot,
                FM1CodeSite = c.FM1?.CodeSite ?? string.Empty,
                FM1DeviceType = c.FM1?.DeviceType ?? string.Empty,
                FM1PsSn = c.FM1?.PsSn ?? string.Empty
            }).ToList();

            return Ok(commandeDtos);
        }

        [HttpGet("commande/{id}")]
        public async Task<IActionResult> GetCommandeById(int id)
        {
            _logger.LogInformation($"Récupération de la Commande avec l'ID : {id}.");

            var result = await _getCommandeByIdQueryHandler.Handle(new GetCommandeByIdQuery(id));

            if (result == null)
            {
                _logger.LogWarning($"Aucune Commande trouvée avec l'ID : {id}.");
                return NotFound();
            }

            var commandeDto = new CommandeDTO
            {
                Id = result.Id,
                EtatCommande = result.EtatCommande,
                DateCmd = result.DateCmd,
                ComposentId = result.ComposentId,
                ExpertId = result.ExpertId,
                RaisonDeCommande = result.RaisonDeCommande,
                FM1Id = result.FM1Id,
                FM1HistoryId = result.FM1HistoryId
            };

            return Ok(commandeDto);
        }

     [HttpGet("fm1histories")]
public async Task<IActionResult> GetAllFM1Histories()
{
    _logger.LogInformation("Récupération de tous les FM1Histories.");
    var result = await _getAllFM1HistoriesQueryHandler.Handle(new GetAllFM1HistoriesQuery());

    if (result == null || !result.Any())
    {
        _logger.LogWarning("Aucun FM1History trouvé.");
        return NotFound();
    }

    var fm1HistoryDtos = result.Select(fh => new FM1HistoryDTO
    {
        Id = fh.Id,
        FM1Id = fh.FM1Id,
        FM1CodeSite = fh.FM1?.CodeSite ?? string.Empty,
        FM1DeviceType = fh.FM1?.DeviceType ?? string.Empty,
        FM1PsSn = fh.FM1?.PsSn ?? string.Empty,

        Commandes = fh.Commandes?.Select(c => new CommandeDTO
        {
                Id = c.Id,
                EtatCommande = c.EtatCommande,
                DateCmd = c.DateCmd,
                ComposentId = c.ComposentId,
                ExpertId = c.ExpertId,
                RaisonDeCommande = c.RaisonDeCommande,
                FM1Id = c.FM1Id,
                FM1HistoryId = c.FM1HistoryId
        }).ToList() ?? new List<CommandeDTO>()
    }).ToList();

    return Ok(fm1HistoryDtos);
}

[HttpGet("fm1histories/{fm1Id}")]
public async Task<IActionResult> GetFM1HistoryByFM1Id(Guid fm1Id)
{
    _logger.LogInformation($"Récupération du FM1History pour le FM1 avec l'ID : {fm1Id}.");
    var result = await _getFM1HistoryByFM1IdQueryHandler.Handle(new GetFM1HistoryByFM1IdQuery { FM1Id = fm1Id });

    if (result == null)
    {
        _logger.LogWarning($"Aucun FM1History trouvé pour le FM1 avec l'ID : {fm1Id}.");
        return NotFound();
    }

    var fm1HistoryDto = new FM1HistoryDTO
    {
        Id = result.Id,
        FM1Id = result.FM1Id,
        FM1CodeSite = result.FM1?.CodeSite ?? string.Empty,
        FM1DeviceType = result.FM1?.DeviceType ?? string.Empty,
        FM1PsSn = result.FM1?.PsSn ?? string.Empty,
        Commandes = result.Commandes?.Select(c => new CommandeDTO
        {
            Id = c.Id,
            EtatCommande = c.EtatCommande,
            DateCmd = c.DateCmd,
            ComposentId = c.ComposentId,
            ExpertId = c.ExpertId,
            RaisonDeCommande = c.RaisonDeCommande,
            FM1Id = c.FM1Id,
            FM1HistoryId = c.FM1HistoryId,
            ComposantProductName = c.Composent?.ProductName ?? string.Empty,
            ComposantSN = c.Composent?.SN,
            ComposantUrgentOrNot = c.Composent?.UrgentOrNot ?? string.Empty,
            ComposantOrderOrNot = c.Composent?.OrderOrNot,
            ExpertNom = c.Expert?.Nom ?? string.Empty,  // Mapping du nom de l'expert
            ExpertPrenom = c.Expert?.Prenom ?? string.Empty // Mapping du prénom de l'expert
        }).ToList() ?? new List<CommandeDTO>()
    };

    return Ok(fm1HistoryDto);
}




    }
}