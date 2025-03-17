using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Models;

namespace GestionFM1.Read.QueryDataStore
{
    public class QueryDbContext : IdentityDbContext<User>
    {
        public DbSet<FM1> FM1s { get; set; }
        public DbSet<Composent> Composents { get; set; }
        public DbSet<Commande> Commandes { get; set; }

        public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FM1>().ToTable("FM1s");
            modelBuilder.Entity<Composent>().ToTable("Composents");
            modelBuilder.Entity<Commande>().ToTable("Commandes");

            // Configuration explicite de la propriété Id pour FM1
            modelBuilder.Entity<FM1>()
                .HasKey(f => f.Id);
            modelBuilder.Entity<FM1>()
                .Property(f => f.Id)
                .IsRequired();

            // Relation entre FM1 et Composent (One-to-Many)
            modelBuilder.Entity<Composent>()
                .HasOne(c => c.FM1)
                .WithMany(f => f.Composents) // Utilisez la propriété de navigation
                .HasForeignKey(c => c.FM1Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation entre FM1 et User (One-to-Many) : Un utilisateur (expert) peut avoir plusieurs FM1s
            modelBuilder.Entity<FM1>()
                .HasOne(f => f.Expert)
                .WithMany(u => u.FM1s)
                .HasForeignKey(f => f.ExpertId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Composent)
                .WithOne(co => co.Commande)
                .HasForeignKey<Commande>(c => c.ComposentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation entre User et Commande (One-to-Many)
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Expert)
                .WithMany(u => u.Commandes)
                .HasForeignKey(c => c.ExpertId)
                .OnDelete(DeleteBehavior.Restrict);

            // Modification ici : ON DELETE NO ACTION
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.FM1)
                .WithMany(f => f.Commandes)
                .HasForeignKey(c => c.FM1Id)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}