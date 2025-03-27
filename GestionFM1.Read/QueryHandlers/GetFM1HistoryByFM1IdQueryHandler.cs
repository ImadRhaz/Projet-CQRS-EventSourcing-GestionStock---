using GestionFM1.Core.Models;
using GestionFM1.Core.Interfaces;
using System.Threading.Tasks;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetFM1HistoryByFM1IdQueryHandler : IQueryHandler<GetFM1HistoryByFM1IdQuery, FM1History>
    {
        private readonly IFM1HistoryReadRepository _fm1HistoryReadRepository;
        private readonly ILogger<GetFM1HistoryByFM1IdQueryHandler> _logger;

        public GetFM1HistoryByFM1IdQueryHandler(IFM1HistoryReadRepository fm1HistoryReadRepository, ILogger<GetFM1HistoryByFM1IdQueryHandler> logger)
        {
            _fm1HistoryReadRepository = fm1HistoryReadRepository;
            _logger = logger;
        }

        public async Task<FM1History> Handle(GetFM1HistoryByFM1IdQuery query)
        {
            _logger.LogInformation($"Récupération du FM1History pour le FM1 avec l'ID : {query.FM1Id}.");
            var fm1History = await _fm1HistoryReadRepository.GetFM1HistoryByFM1IdAsync(query.FM1Id);

            if (fm1History == null)
            {
                _logger.LogWarning($"Aucun FM1History trouvé pour le FM1 avec l'ID : {query.FM1Id}.");
            }

            return fm1History;
        }
    }
}