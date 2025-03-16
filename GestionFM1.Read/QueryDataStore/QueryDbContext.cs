using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Models; // Ajoute cette ligne

namespace GestionFM1.Read.QueryDataStore
{
    public class QueryDbContext : IdentityDbContext<User>
    {
        public DbSet<FM1> FM1s { get; set; }

        public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FM1>().ToTable("FM1s");

            // Configuration explicite de la propriété Id
            modelBuilder.Entity<FM1>()
                .HasKey(i => i.Id); // Définir Id comme clé primaire
            modelBuilder.Entity<FM1>()
                .Property(i => i.Id)
                .IsRequired(); // Id est requis
            modelBuilder.Entity<FM1>()
               .HasOne(f => f.Expert)
               .WithMany()
               .HasForeignKey(f => f.ExpertId);
        }
    }
}