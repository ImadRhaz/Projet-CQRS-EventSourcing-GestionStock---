using System;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetComposentByIdQueryHandler : IQueryHandler<GetComposentByIdQuery, Composent>
    {
        private readonly IComposentReadRepository _composentReadRepository;
        private readonly ILogger<GetComposentByIdQueryHandler> _logger;

        public GetComposentByIdQueryHandler(IComposentReadRepository composentReadRepository, ILogger<GetComposentByIdQueryHandler> logger)
        {
            _composentReadRepository = composentReadRepository;
            _logger = logger;
        }

        public async Task<Composent> Handle(GetComposentByIdQuery query)
        {
            _logger.LogInformation($"Récupération du Composent avec l'ID : {query.Id}.");
            var composent = await _composentReadRepository.GetComposentByIdAsync(query.Id);

            if (composent == null)
            {
                _logger.LogWarning($"Aucun Composent trouvé avec l'ID : {query.Id}.");
            }

            return composent;
        }
    }
}