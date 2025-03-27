using GestionFM1.Core.Models;
using System.Threading.Tasks;

namespace GestionFM1.Core.Interfaces
{
    public interface IUserReadRepository
    {
       
        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}