using System;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetFM1ByIdQueryHandler : IQueryHandler<GetFM1ByIdQuery, FM1>
    {
        private readonly IFM1ReadRepository _fm1ReadRepository;
        private readonly ILogger<GetFM1ByIdQueryHandler> _logger;

        public GetFM1ByIdQueryHandler(IFM1ReadRepository fm1ReadRepository, ILogger<GetFM1ByIdQueryHandler> logger)
        {
            _fm1ReadRepository = fm1ReadRepository;
            _logger = logger;
        }

        public async Task<FM1> Handle(GetFM1ByIdQuery query)
        {
            _logger.LogInformation($"Récupération du FM1 avec l'ID : {query.Id}.");
            var fm1 = await _fm1ReadRepository.GetFM1ByIdAsync(query.Id);

            if (fm1 == null)
            {
                _logger.LogWarning($"Aucun FM1 trouvé avec l'ID : {query.Id}.");
            }

            return fm1;
        }
    }
}