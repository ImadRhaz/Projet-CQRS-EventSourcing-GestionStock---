using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetAllCommandesQueryHandler : IQueryHandler<GetAllCommandesQuery, IEnumerable<Commande>>
    {
        private readonly ICommandeReadRepository _commandeReadRepository;
        private readonly ILogger<GetAllCommandesQueryHandler> _logger;

        public GetAllCommandesQueryHandler(ICommandeReadRepository commandeReadRepository, ILogger<GetAllCommandesQueryHandler> logger)
        {
            _commandeReadRepository = commandeReadRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Commande>> Handle(GetAllCommandesQuery query)
        {
            _logger.LogInformation("Récupération de toutes les Commandes.");
            var commandes = await _commandeReadRepository.GetAllCommandesAsync();

            if (commandes == null || !commandes.Any())
            {
                _logger.LogWarning("Aucune Commande trouvée.");
            }

            return commandes;
        }
    }
}