using GestionFM1.Core.Models;
using GestionFM1.Read.QueryDataStore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Interfaces;

namespace GestionFM1.Read.Repositories
{
    public class UserReadRepository : IUserReadRepository
    {
        private readonly QueryDbContext _queryDbContext;

        public UserReadRepository(QueryDbContext queryDbContext)
        {
            _queryDbContext = queryDbContext;
        }

        

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            return new List<string>();
        }

       
    }
}