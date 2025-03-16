using Microsoft.AspNetCore.Mvc;
using GestionFM1.Read.Queries;
using GestionFM1.Core.Interfaces;
using System.Threading.Tasks;
using GestionFM1.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using GestionFM1.Read.QueryHandlers;

namespace GestionFM1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> _logger;
        private readonly IQueryHandler<GetUserRolesQuery, IList<string>> _getUserRolesQueryHandler;

        public QueryController(
            IQueryHandler<GetUserRolesQuery, IList<string>> getUserRolesQueryHandler,
            ILogger<QueryController> logger)
        {
            _loginQueryHandler = loginQueryHandler;
            _getUserRolesQueryHandler = getUserRolesQueryHandler;
            _logger = logger;
        }

        [AllowAnonymous] // Add this attribute
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)  // Check if the model is valid
            {
                _logger.LogWarning("Tentative de connexion avec un modèle invalide.");
                return BadRequest(ModelState); // Return validation errors
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

    }
}