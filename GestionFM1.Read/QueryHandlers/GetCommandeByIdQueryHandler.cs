using System;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetCommandeByIdQueryHandler : IQueryHandler<GetCommandeByIdQuery, Commande>
    {
        private readonly ICommandeReadRepository _commandeReadRepository;
        private readonly ILogger<GetCommandeByIdQueryHandler> _logger;

        public GetCommandeByIdQueryHandler(ICommandeReadRepository commandeReadRepository, ILogger<GetCommandeByIdQueryHandler> logger)
        {
            _commandeReadRepository = commandeReadRepository;
            _logger = logger;
        }

        public async Task<Commande> Handle(GetCommandeByIdQuery query)
        {
            _logger.LogInformation($"Récupération de la Commande avec l'ID : {query.Id}.");
            var commande = await _commandeReadRepository.GetCommandeByIdAsync(query.Id);

            if (commande == null)
            {
                _logger.LogWarning($"Aucune Commande trouvée avec l'ID : {query.Id}.");
            }

            return commande;
        }
    }
}