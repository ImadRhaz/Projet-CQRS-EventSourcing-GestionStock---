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
    public class CommandeReadRepository : ICommandeReadRepository
    {
        private readonly QueryDbContext _context;
        private readonly ILogger<CommandeReadRepository> _logger;

        public CommandeReadRepository(QueryDbContext context, ILogger<CommandeReadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Commande> GetCommandeByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Récupération de la Commande avec l'ID : {id}.");
                var commande = await _context.Commandes
                    .Include(c => c.Composent) // Inclure Composent
                    .Include(c => c.Expert) // Inclure Expert
                    .Include(c => c.FM1) // Inclure FM1
                    .Include(c => c.FM1History) // Inclure FM1History
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (commande == null)
                {
                    _logger.LogWarning($"Aucune Commande trouvée avec l'ID : {id}.");
                }

                return commande;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de la Commande avec l'ID : {id}.");
                throw;
            }
        }

        public async Task<IEnumerable<Commande>> GetAllCommandesAsync()
        {
            try
            {
                _logger.LogInformation("Récupération de toutes les Commandes.");
                var commandes = await _context.Commandes
                    .Include(c => c.Composent) // Inclure Composent
                    .Include(c => c.Expert) // Inclure Expert
                    .Include(c => c.FM1) // Inclure FM1
                    .Include(c => c.FM1History) // Inclure FM1History
                    .ToListAsync();

                return commandes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les Commandes.");
                throw;
            }
        }
    }
}