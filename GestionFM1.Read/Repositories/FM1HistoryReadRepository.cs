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
                    .Include(fh => fh.Commandes)
                        .ThenInclude(c => c.Expert)
                    .Include(fh => fh.FM1)
                    .ToListAsync();

                return fm1Histories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les FM1Histories.");
                throw; // Ou gérer l'exception différemment si nécessaire
            }
        }

        public async Task<FM1History> GetFM1HistoryByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation($"Récupération du FM1History avec l'ID : {id}.");
                var fm1History = await _context.FM1Histories
                    .Include(fh => fh.Commandes)
                        .ThenInclude(c => c.Expert)
                    .Include(fh => fh.FM1)
                    .FirstOrDefaultAsync(fh => fh.Id == id);

                return fm1History;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération du FM1History avec l'ID : {id}.");
                throw; // Ou gérer l'exception différemment si nécessaire
            }
        }

        public async Task<FM1History> GetFM1HistoryByFM1IdAsync(Guid fm1Id)
        {
            try
            {
                _logger.LogInformation($"Récupération du FM1History pour le FM1 avec l'ID : {fm1Id}.");
                var fm1History = await _context.FM1Histories
                    .Include(fh => fh.FM1) // Inclure FM1 directement
                    .Include(fh => fh.Commandes)
                        .ThenInclude(c => c.Composent)
                        .ThenInclude(comp => comp.FM1) // Inclure le FM1 du composant si nécessaire
                        .ThenInclude(fm1 => fm1.Expert) //Expert relié à FM1 du composant si nécessaire
                    .Include(fh => fh.Commandes) //Re-inclure Commandes pour inclure l'expert directement lié à la commande
                        .ThenInclude(c => c.Expert) // Inclure Expert de la Commande ici
                    .FirstOrDefaultAsync(fh => fh.FM1Id == fm1Id);

                return fm1History;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération du FM1History pour le FM1 avec l'ID : {fm1Id}.");
                throw; // Ou gérer l'exception différemment si nécessaire
            }
        }




        
    }
}