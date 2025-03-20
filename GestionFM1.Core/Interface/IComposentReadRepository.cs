using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionFM1.Core.Models;

namespace GestionFM1.Core.Interfaces
{
    public interface IComposentReadRepository
    {
        Task<Composent> GetComposentByIdAsync(Guid id);
        Task<IEnumerable<Composent>> GetAllComposentsAsync();
        Task<IEnumerable<Composent>> GetComposentsByFM1IdAsync(Guid fm1Id);  // Added

    }
}