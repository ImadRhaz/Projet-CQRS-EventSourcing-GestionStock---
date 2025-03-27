using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionFM1.Core.Models;

namespace GestionFM1.Core.Interfaces
{
    public interface IFM1HistoryReadRepository
    {
        Task<IEnumerable<FM1History>> GetAllFM1HistoriesAsync();
                Task<FM1History> GetFM1HistoryByIdAsync(Guid id);
             Task<FM1History> GetFM1HistoryByFM1IdAsync(Guid fm1Id); // New Method


        
    }
}