using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetAllComposentsQueryHandler : IQueryHandler<GetAllComposentsQuery, IEnumerable<Composent>>
    {
        private readonly IComposentReadRepository _composentReadRepository;
        private readonly ILogger<GetAllComposentsQueryHandler> _logger;

        public GetAllComposentsQueryHandler(IComposentReadRepository composentReadRepository, ILogger<GetAllComposentsQueryHandler> logger)
        {
            _composentReadRepository = composentReadRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Composent>> Handle(GetAllComposentsQuery query)
        {
            _logger.LogInformation("Récupération de tous les Composents.");
            var composants = await _composentReadRepository.GetAllComposentsAsync();

            if (composants == null || !composants.Any())
            {
                _logger.LogWarning("Aucun Composent trouvé.");
            }

            return composants;
        }
    }
}