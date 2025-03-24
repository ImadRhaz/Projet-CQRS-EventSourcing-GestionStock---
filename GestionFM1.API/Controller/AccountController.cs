using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GestionFM1.Core.Models;
using GestionFM1.DTOs;
using GestionFM1.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;
using System.Security.Claims;
using OtpNet; // Ajout de l'espace de noms OtpNet

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

        // Méthode pour générer un token JWT et retourner la réponse
        private async Task<IActionResult> GenerateJwtTokenAndReturn(User user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation("Generating JWT for user {UserId} with roles: {Roles}", user.Id, string.Join(",", roles));

                var token = JwtUtils.GenerateJwtToken(user, _configuration, roles);

                if (token != null)
                {
                    _logger.LogInformation("Successful login for {UserId} with token: {Token}", user.Id, token);
                    return Ok(new { Token = token });
                }

                _logger.LogError("Error generating JWT token for {UserId}", user.Id);
                return Unauthorized("Error generating authentication token.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la génération du token JWT.");
                return StatusCode(500, "Une erreur interne s'est produite.");
            }
        }

        // Endpoint pour la connexion
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Vérifier si le DTO est null
            if (loginDto == null)
            {
                _logger.LogWarning("LoginDTO est null.");
                return BadRequest("LoginDTO est null.");
            }

            _logger.LogInformation("Login request received for {Email}", loginDto.Email);

            // Validation du modèle
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login request: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            // Trouver l'utilisateur par email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", loginDto.Email);
                return Unauthorized("Invalid login attempt.");
            }

            // Vérifier le mot de passe
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Incorrect password for {Email}", loginDto.Email);
                return Unauthorized("Invalid login attempt.");
            }

            _logger.LogInformation("User {Email} authenticated successfully. Checking 2FA...", loginDto.Email);

            // Vérifier si la 2FA est activée
            var is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (is2faEnabled)
            {
                _logger.LogInformation("Two-factor authentication required for {Email}", loginDto.Email);

                // Vérifier que la clé d'authentification est configurée
                var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(authenticatorKey))
                {
                    _logger.LogWarning("Aucune clé d'authentification trouvée pour l'utilisateur : {Email}", loginDto.Email);
                    return BadRequest("Aucune clé d'authentification trouvée.");
                }

                // Générer le code 2FA attendu pour débogage
                var expectedCode = await _userManager.GenerateTwoFactorTokenAsync(
                    user,
                    _userManager.Options.Tokens.AuthenticatorTokenProvider
                );

                // Afficher le code attendu dans les logs
                _logger.LogInformation("Code 2FA attendu pour {Email} : {ExpectedCode}", loginDto.Email, expectedCode);

                return Ok(new { TwoFactorRequired = true, Email = user.Email });
            }

            // Si la 2FA n'est pas activée, générer un token JWT
            return await GenerateJwtTokenAndReturn(user);
        }
[AllowAnonymous]
[HttpPost("login-2fa")]
public async Task<IActionResult> Login2fa([FromBody] Login2faDTO login2faDto)
{
    _logger.LogInformation("Reçu une demande de connexion 2FA pour l'e-mail : {Email}", login2faDto.Email);

    // Validation du modèle
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("État du modèle invalide pour la demande de connexion 2FA : {@ModelState}", ModelState);
        return BadRequest(ModelState);
    }

    // Trouver l'utilisateur par e-mail
    var user = await _userManager.FindByEmailAsync(login2faDto.Email);
    if (user == null)
    {
        _logger.LogWarning("Utilisateur non trouvé pour l'e-mail : {Email}", login2faDto.Email);
        return Unauthorized("Utilisateur non trouvé.");
    }

    // Vérifier que la 2FA est activée pour l'utilisateur
    var is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
    _logger.LogInformation("2FA activée pour l'utilisateur {Email}: {Is2faEnabled}", user.Email, is2faEnabled);
    if (!is2faEnabled)
    {
        _logger.LogWarning("La 2FA n'est pas activée pour l'utilisateur : {Email}", user.Email);
        return BadRequest("La 2FA n'est pas activée pour cet utilisateur.");
    }

    // Vérifier que la clé d'authentification est configurée
    var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
    _logger.LogInformation("Clé d'authentification récupérée de la base de données : {AuthenticatorKey}", authenticatorKey);
    if (string.IsNullOrEmpty(authenticatorKey))
    {
        _logger.LogWarning("Aucune clé d'authentification trouvée pour l'utilisateur : {Email}", user.Email);
        return BadRequest("Aucune clé d'authentification trouvée.");
    }

    // Convertir la clé d'authentification en tableau de bytes
    var keyBytes = Base32Encoding.ToBytes(authenticatorKey);  // Utilisation de Base32Encoding pour convertir la clé

    // Créer une instance de Totp avec la clé
    var totp = new Totp(keyBytes, mode: OtpHashMode.Sha1, step: 30, totpSize: 6);

    // Générer le code attendu pour débogage
    var expectedCode = totp.ComputeTotp();  // Génère le code attendu
    _logger.LogInformation("Code attendu (Otp.NET) : {ExpectedCode}", expectedCode);

    // Créer une fenêtre de vérification avec une tolérance de ±5 pas de temps
    var verificationWindow = new VerificationWindow(previous: 5, future: 5);

    // À ce point, on ignore la vérification du code et on accepte tous les codes comme valides
    var isValid = true;  // Accepte tous les codes fournis comme valides, quel que soit leur contenu

    // Log du résultat de la vérification
    _logger.LogInformation("Utilisateur : {Email}, Code fourni : {ProvidedCode}, Vérification Otp.NET : {IsValid}",
        user.Email, login2faDto.TwoFactorCode, isValid);

    // Si la vérification 2FA est réussie (ou ignorée ici)
    if (!isValid)
    {
        _logger.LogWarning("Code 2FA invalide (vérification Otp.NET) pour l'utilisateur : {Email}", user.Email);
        return Unauthorized("Code 2FA invalide.");
    }

    // Générer un token JWT pour l'utilisateur
    _logger.LogInformation("Code 2FA valide pour l'utilisateur : {Email}. Génération du token JWT...", user.Email);
    return await GenerateJwtTokenAndReturn(user);
}

        // Endpoint pour activer la 2FA
        [AllowAnonymous]
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> Enable2fa([FromBody] Enable2faRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                _logger.LogError("Email not provided in Enable2FA request");
                return BadRequest("Email is required.");
            }

            // Récupérer l'utilisateur par son email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogError("User not found for email {Email}", request.Email);
                return NotFound("User not found.");
            }

            _logger.LogInformation("Enabling 2FA for user {UserId}", user.Id);

            // Réinitialiser la clé d'authentification
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(sharedKey))
            {
                _logger.LogError("Impossible de générer une clé d'authentificateur pour {UserId}", user.Id);
                return BadRequest("Could not generate authenticator key.");
            }

            // Générer l'URI et le QR code
            var authenticatorUri = GenerateQrCodeUri(user.Email, sharedKey);
            var qrCodeDataUri = GenerateQrCodeDataUri(authenticatorUri);

            // Activer la 2FA pour l'utilisateur
            await _userManager.SetTwoFactorEnabledAsync(user, true);

            _logger.LogInformation("2FA activée pour {UserId}", user.Id);

            // Retourner la clé partagée et le QR code
            return Ok(new { SharedKey = sharedKey, AuthenticatorUri = authenticatorUri, QrCodeDataUri = qrCodeDataUri });
        }

        // Classe pour représenter la requête d'activation de la 2FA
        public class Enable2faRequest
        {
            public string Email { get; set; } = string.Empty; // Initialisation par défaut
        }

        // Endpoint pour désactiver la 2FA
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

        // Méthode pour générer l'URI du QR code
        private string GenerateQrCodeUri(string userName, string authenticatorKey)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName), "Le nom d'utilisateur ne peut pas être null ou vide.");
            }

            if (string.IsNullOrEmpty(authenticatorKey))
            {
                throw new ArgumentNullException(nameof(authenticatorKey), "La clé d'authentification ne peut pas être null ou vide.");
            }

            return $"otpauth://totp/GestionFM1:{userName}?secret={authenticatorKey}&issuer=GestionFM1";
        }

        // Méthode pour générer le QR code en base64
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