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
    public class FM1ReadRepository : IFM1ReadRepository
    {
        private readonly QueryDbContext _context;
        private readonly ILogger<FM1ReadRepository> _logger;

        public FM1ReadRepository(QueryDbContext context, ILogger<FM1ReadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FM1> GetFM1ByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Récupération de FM1 avec l'ID : {id}.");
                var fm1 = await _context.FM1s
                    .Include(f => f.Expert)  // Inclure l'expert
                    .Include(f => f.FM1History) // Inclure FM1History
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (fm1 == null)
                {
                    _logger.LogWarning($"Aucun FM1 trouvé avec l'ID : {id}.");
                }

                return fm1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de FM1 avec l'ID : {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<FM1>> GetAllFM1Async()
        {
            try
            {
                _logger.LogInformation("Récupération de tous les FM1.");
                var fm1s = await _context.FM1s
                    .Include(f => f.Expert) // Inclure l'expert
                    .Include(f => f.FM1History) // Inclure FM1History
                    .ToListAsync();

                return fm1s;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les FM1.");
                throw;
            }
        }
          


        
    }
}