using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetAllFM1QueryHandler : IQueryHandler<GetAllFM1Query, IEnumerable<FM1>>
    {
        private readonly IFM1ReadRepository _fm1ReadRepository;
        private readonly ILogger<GetAllFM1QueryHandler> _logger;

        public GetAllFM1QueryHandler(IFM1ReadRepository fm1ReadRepository, ILogger<GetAllFM1QueryHandler> logger)
        {
            _fm1ReadRepository = fm1ReadRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<FM1>> Handle(GetAllFM1Query query)
        {
            _logger.LogInformation("Récupération de tous les FM1.");
            var fm1s = await _fm1ReadRepository.GetAllFM1Async();

            if (fm1s == null || !fm1s.Any())
            {
                _logger.LogWarning("Aucun FM1 trouvé.");
            }

            return fm1s;
        }
    }
}