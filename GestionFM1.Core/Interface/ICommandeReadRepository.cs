using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionFM1.Core.Models;

namespace GestionFM1.Core.Interfaces
{
    public interface ICommandeReadRepository
    {
        Task<Commande> GetCommandeByIdAsync(int id); // L'ID est un int ici
        Task<IEnumerable<Commande>> GetAllCommandesAsync();
    }
}