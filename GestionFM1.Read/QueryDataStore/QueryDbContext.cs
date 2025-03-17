using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Models;

namespace GestionFM1.Read.QueryDataStore
{
    public class QueryDbContext : IdentityDbContext<User>
    {
        public DbSet<FM1> FM1s { get; set; }
        public DbSet<Composent> Composents { get; set; }

        public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FM1>().ToTable("FM1s");
            modelBuilder.Entity<Composent>().ToTable("Composents");

            // Configuration explicite de la propriété Id
            modelBuilder.Entity<FM1>()
                .HasKey(f => f.Id); // Définir Id comme clé primaire
            modelBuilder.Entity<FM1>()
                .Property(f => f.Id)
                .IsRequired(); // Id est requis

            // Relation entre FM1 et Composent
            modelBuilder.Entity<Composent>()
                .HasOne(c => c.FM1)
                .WithMany()
                .HasForeignKey(c => c.FM1Id);
        }
    }
}