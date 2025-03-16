using Microsoft.AspNetCore.Mvc;
using GestionFM1.Write.Commands;
using GestionFM1.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GestionFM1.DTOs;

namespace GestionFM1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly RabbitMqCommandBus _commandBus;
        private readonly ILogger<CommandController> _logger;

        public CommandController(RabbitMqCommandBus commandBus, ILogger<CommandController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

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
    }
}