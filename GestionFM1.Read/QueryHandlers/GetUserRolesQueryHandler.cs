using GestionFM1.Core.Interfaces; // Ajoutez cette ligne!
using GestionFM1.Read.Queries;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionFM1.Core.Models;  // Important : pour la classe User
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetUserRolesQueryHandler : IQueryHandler<GetUserRolesQuery, IList<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<GetUserRolesQueryHandler> _logger;

        public GetUserRolesQueryHandler(UserManager<User> userManager, ILogger<GetUserRolesQueryHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IList<string>> Handle(GetUserRolesQuery query)
        {
            _logger.LogInformation($"Récupération des rôles pour l'utilisateur avec l'ID : {query.UserId}");

            var user = await _userManager.FindByIdAsync(query.UserId);

            if (user == null)
            {
                _logger.LogWarning($"Utilisateur avec l'ID : {query.UserId} non trouvé.");
                return new List<string>(); // Retourner une liste vide si l'utilisateur n'existe pas
            }

            var roles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation($"Rôles récupérés pour l'utilisateur avec l'ID : {query.UserId}: {string.Join(", ", roles)}");
            return roles;
        }
    }
}