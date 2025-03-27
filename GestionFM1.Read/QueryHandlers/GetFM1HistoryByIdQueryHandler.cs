using GestionFM1.Core.Models;
using GestionFM1.Core.Interfaces;
using System.Threading.Tasks;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetFM1HistoryByIdQueryHandler : IQueryHandler<GetFM1HistoryByIdQuery, FM1History>
    {
        private readonly IFM1HistoryReadRepository _fm1HistoryReadRepository;
        private readonly ILogger<GetFM1HistoryByIdQueryHandler> _logger;

        public GetFM1HistoryByIdQueryHandler(IFM1HistoryReadRepository fm1HistoryReadRepository, ILogger<GetFM1HistoryByIdQueryHandler> logger)
        {
            _fm1HistoryReadRepository = fm1HistoryReadRepository;
            _logger = logger;
        }

        public async Task<FM1History> Handle(GetFM1HistoryByIdQuery query)
        {
            _logger.LogInformation($"Récupération du FM1History avec l'ID : {query.Id}.");
            var fm1History = await _fm1HistoryReadRepository.GetFM1HistoryByIdAsync(query.Id);

            if (fm1History == null)
            {
                _logger.LogWarning($"Aucun FM1History trouvé avec l'ID : {query.Id}.");
            }

            return fm1History;
        }
    }
}