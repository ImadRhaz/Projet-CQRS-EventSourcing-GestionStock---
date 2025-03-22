using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GestionFM1.Core.Models;
using GestionFM1.DTOs;
using GestionFM1.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using QRCoder;

namespace GestionFM1.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<User> userManager, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            _logger.LogInformation("Received login request for {Email}", loginDto.Email); // Log request received

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login request: {@ModelState}", ModelState); // Log model state errors
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                _logger.LogWarning("Invalid login attempt for {Email}", loginDto.Email); // Log invalid login attempt
                return Unauthorized("Invalid login attempt.");
            }

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                _logger.LogInformation("Two-factor authentication required for {Email}", loginDto.Email); // Log 2FA required
                return Ok(new { TwoFactorRequired = true, Email = user.Email });
            }

            return await GenerateJwtTokenAndReturn(user);
        }

        [AllowAnonymous]
        [HttpPost("login-2fa")]
        public async Task<IActionResult> Login2fa([FromBody] Login2faDTO login2faDto)
        {
            _logger.LogInformation("Received 2FA login request for {Email}", login2faDto.Email); // Log 2FA request

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for 2FA login request: {@ModelState}", ModelState); // Log model state errors
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(login2faDto.Email);
            if (user == null || !await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", login2faDto.TwoFactorCode))
            {
                _logger.LogWarning("Invalid 2FA code for {Email}", login2faDto.Email); // Log invalid 2FA
                return Unauthorized("Invalid 2FA code.");
            }

            return await GenerateJwtTokenAndReturn(user);
        }

        private async Task<IActionResult> GenerateJwtTokenAndReturn(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = JwtUtils.GenerateJwtToken(user, _configuration, roles);

            if (token != null)
            {
                _logger.LogInformation("Successful login for {UserId}", user.Id); // Log successful login
                return Ok(new { Token = token });
            }

            _logger.LogError("Error generating JWT token for {UserId}", user.Id); // Log token generation error
            return Unauthorized("Error generating authentication token.");
        }

        [Authorize]
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> Enable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Utilisateur introuvable pour l'activation de la 2FA");
                return NotFound("User not found.");
            }

            await _userManager.ResetAuthenticatorKeyAsync(user);
            var sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(sharedKey))
            {
                _logger.LogError("Impossible de générer une clé d'authentificateur pour {UserId}", user.Id);
                return BadRequest("Could not generate authenticator key.");
            }

            var authenticatorUri = GenerateQrCodeUri(user.Email, sharedKey);
            var qrCodeDataUri = GenerateQrCodeDataUri(authenticatorUri);

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            _logger.LogInformation("2FA activée pour {UserId}", user.Id);

            return Ok(new { SharedKey = sharedKey, AuthenticatorUri = authenticatorUri, QrCodeDataUri = qrCodeDataUri });
        }

        [Authorize]
        [HttpPost("disable-2fa")]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Utilisateur introuvable pour la désactivation de la 2FA");
                return NotFound("User not found.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            _logger.LogInformation("2FA désactivée pour {UserId}", user.Id);
            return Ok("2FA disabled successfully.");
        }

        private string GenerateQrCodeUri(string userName, string authenticatorKey)
        {
            return $"otpauth://totp/GestionFM1:{userName}?secret={authenticatorKey}&issuer=GestionFM1";
        }

        private string GenerateQrCodeDataUri(string qrCodeText)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        var qrCodeBytes = qrCode.GetGraphic(20);
                        return "data:image/png;base64," + Convert.ToBase64String(qrCodeBytes);
                    }
                }
            }
        }
    }
}