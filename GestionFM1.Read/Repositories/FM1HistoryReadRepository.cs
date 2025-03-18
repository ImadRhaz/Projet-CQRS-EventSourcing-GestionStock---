using GestionFM1.Core.Models;
using GestionFM1.Read.QueryDataStore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionFM1.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionFM1.Read.Repositories
{
    public class FM1HistoryReadRepository : IFM1HistoryReadRepository
    {
        private readonly QueryDbContext _context;
        private readonly ILogger<FM1HistoryReadRepository> _logger;

        public FM1HistoryReadRepository(QueryDbContext context, ILogger<FM1HistoryReadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<FM1History>> GetAllFM1HistoriesAsync()
        {
            try
            {
                _logger.LogInformation("Récupération de tous les FM1Histories.");
                var fm1Histories = await _context.FM1Histories
                    .Include(fh => fh.FM1) // Inclure FM1
                    .ToListAsync();

                return fm1Histories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les FM1Histories.");
                throw;
            }
        }
        
    }
}