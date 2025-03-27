// GetComposentsByFM1IdQueryHandler.cs
using GestionFM1.Core.Interfaces;
using GestionFM1.Core.Models;
using GestionFM1.Read.Queries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionFM1.Read.QueryHandlers
{
    public class GetComposentsByFM1IdQueryHandler : IQueryHandler<GetComposentsByFM1IdQuery, IEnumerable<Composent>>
    {
        private readonly IComposentReadRepository _composentReadRepository;
        private readonly ILogger<GetComposentsByFM1IdQueryHandler> _logger;

        public GetComposentsByFM1IdQueryHandler(IComposentReadRepository composentReadRepository, ILogger<GetComposentsByFM1IdQueryHandler> logger)
        {
            _composentReadRepository = composentReadRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Composent>> Handle(GetComposentsByFM1IdQuery query)
        {
            try
            {
                _logger.LogInformation($"Récupération des Composents pour le FM1 avec l'ID : {query.FM1Id}.");
                var composants = await _composentReadRepository.GetComposentsByFM1IdAsync(query.FM1Id);
                return composants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération des Composents pour le FM1 avec l'ID : {query.FM1Id}.");
                throw;
            }
        }
    }
}