using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Models;

namespace GestionFM1.Read.QueryDataStore
{
    public class QueryDbContext : IdentityDbContext<User>
    {
        public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}