using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using GestionFM1.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GestionFM1.Read.QueryHandlers
{
    public class LoginQueryHandler
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginQueryHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager; // Inject UserManager

        public LoginQueryHandler(SignInManager<User> signInManager, ILogger<LoginQueryHandler> logger, IConfiguration configuration, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string?> Handle(LoginQuery query)
        {
            _logger.LogInformation("Recherche de l'utilisateur avec l'email : {Email}", query.Email);

           
            var user = await _userManager.FindByEmailAsync(query.Email);
             if (user == null)
            {
                _logger.LogWarning("Utilisateur non trouvé : {Email}", query.Email);
                return null; // Important: Return null if user not found
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, query.Password, false); // Use CheckPasswordSignInAsync

            if (result.Succeeded)
            {
                _logger.LogInformation("Connexion réussie pour l'utilisateur : {Email}", query.Email);

                try
                {
                    // Generate the JWT token here:
                    var jwtSettings = _configuration.GetSection("Jwt");

                    // Check if the JWT section exists
                    if (jwtSettings == null)
                    {
                        _logger.LogError("La section 'Jwt' n'est pas configurée dans appsettings.json.");
                        return null; // Exit if configuration is missing
                    }

                    var key = jwtSettings["Key"];
                    var issuer = jwtSettings["Issuer"];
                    var audience = jwtSettings["Audience"];

                    // More robust checks for required settings
                    if (string.IsNullOrEmpty(key))
                    {
                        _logger.LogError("La clé 'Key' n'est pas configurée dans la section 'Jwt' de appsettings.json.");
                        return null;
                    }

                    if (string.IsNullOrEmpty(issuer))
                    {
                        _logger.LogError("L'émetteur 'Issuer' n'est pas configuré dans la section 'Jwt' de appsettings.json.");
                        return null;
                    }

                    if (string.IsNullOrEmpty(audience))
                    {
                        _logger.LogError("L'audience 'Audience' n'est pas configurée dans la section 'Jwt' de appsettings.json.");
                        return null;
                    }

                    var signingKey = Encoding.ASCII.GetBytes(key);

                    // Get roles from UserManager
                    var roles = await _userManager.GetRolesAsync(user);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim(ClaimTypes.Name, user.UserName!)
                    };

                    // Add roles as claims
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature),
                        Issuer = issuer,
                        Audience = audience
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);

                    return jwtToken;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la génération du token JWT pour l'utilisateur : {Email}", query.Email);
                    return null;  // Return null in case of exception during token generation
                }
            }
            else
            {
                _logger.LogWarning("Mot de passe incorrect pour l'utilisateur : {Email}", query.Email);
                return null; // Important: Return null if sign-in fails
            }
        }
    }
}