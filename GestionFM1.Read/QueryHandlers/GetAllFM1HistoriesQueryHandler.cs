using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetAllFM1HistoriesQueryHandler : IQueryHandler<GetAllFM1HistoriesQuery, IEnumerable<FM1History>>
    {
        private readonly IFM1HistoryReadRepository _fm1HistoryReadRepository;
        private readonly ILogger<GetAllFM1HistoriesQueryHandler> _logger;

        public GetAllFM1HistoriesQueryHandler(IFM1HistoryReadRepository fm1HistoryReadRepository, ILogger<GetAllFM1HistoriesQueryHandler> logger)
        {
            _fm1HistoryReadRepository = fm1HistoryReadRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<FM1History>> Handle(GetAllFM1HistoriesQuery query)
        {
            _logger.LogInformation("Récupération de tous les FM1Histories.");
            var fm1Histories = await _fm1HistoryReadRepository.GetAllFM1HistoriesAsync();

            if (fm1Histories == null || !fm1Histories.Any())
            {
                _logger.LogWarning("Aucun FM1History trouvé.");
            }

            return fm1Histories;
        }
    }
}