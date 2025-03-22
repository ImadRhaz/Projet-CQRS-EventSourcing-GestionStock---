using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionFM1.Core.Models;

namespace GestionFM1.Core.Interfaces
{
    public interface IFM1ReadRepository
    {
        Task<FM1> GetFM1ByIdAsync(Guid id);
        Task<IEnumerable<FM1>> GetAllFM1Async();
        Task DeleteFM1ByIdAsync(Guid id); // Add this line

    }
}