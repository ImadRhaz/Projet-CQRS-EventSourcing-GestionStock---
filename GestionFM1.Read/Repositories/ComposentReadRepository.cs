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
    public class ComposentReadRepository : IComposentReadRepository
    {
        private readonly QueryDbContext _context;
        private readonly ILogger<ComposentReadRepository> _logger;

        public ComposentReadRepository(QueryDbContext context, ILogger<ComposentReadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Composent> GetComposentByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Récupération du Composent avec l'ID : {id}.");
                var composent = await _context.Composents
                    .Include(c => c.FM1) // Inclure FM1
                    .Include(c => c.Commande) // Inclure Commande
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (composent == null)
                {
                    _logger.LogWarning($"Aucun Composent trouvé avec l'ID : {id}.");
                }

                return composent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération du Composent avec l'ID : {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<Composent>> GetAllComposentsAsync()
        {
            try
            {
                _logger.LogInformation("Récupération de tous les Composents.");
                var composants = await _context.Composents
                    .Include(c => c.FM1) // Inclure FM1
                    .Include(c => c.Commande) // Inclure Commande
                    .ToListAsync();

                return composants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les Composents.");
                throw;
            }
        }
    }
}