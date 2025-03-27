using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GestionFM1.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionFM1.Read.QueryDataStore
{
    public class QueryDbContext : IdentityDbContext<User>
    {
        public DbSet<FM1> FM1s { get; set; }
        public DbSet<Composent> Composents { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<FM1History> FM1Histories { get; set; }
        public DbSet<ExcelFm1> ExcelFm1s { get; set; }
        public DbSet<ExcelComposent> ExcelComposents { get; set; }

        public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des tables
            modelBuilder.Entity<FM1>().ToTable("FM1s");
            modelBuilder.Entity<Composent>().ToTable("Composents");
            modelBuilder.Entity<Commande>().ToTable("Commandes");
            modelBuilder.Entity<FM1History>().ToTable("FM1Histories");
            modelBuilder.Entity<ExcelFm1>().ToTable("ExcelFm1s");
            modelBuilder.Entity<ExcelComposent>().ToTable("ExcelComposents");

            // Configuration des clés primaires
            modelBuilder.Entity<FM1>().HasKey(f => f.Id);
            modelBuilder.Entity<Composent>().HasKey(c => c.Id);
            modelBuilder.Entity<FM1History>().HasKey(h => h.Id);

            // Configuration spécifique pour Commande
            modelBuilder.Entity<Commande>()
                .HasKey(c => c.Id);
                
            modelBuilder.Entity<Commande>()
                .Property(c => c.Id)
                .UseIdentityColumn(); // Meilleure méthode pour EF Core

            // Relations FM1-Composent (One-to-Many)
            modelBuilder.Entity<Composent>()
                .HasOne(c => c.FM1)
                .WithMany(f => f.Composents)
                .HasForeignKey(c => c.FM1Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relations User-FM1 (One-to-Many)
            modelBuilder.Entity<FM1>()
                .HasOne(f => f.Expert)
                .WithMany(u => u.FM1s)
                .HasForeignKey(f => f.ExpertId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations User-Commande (One-to-Many)
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Expert)
                .WithMany(u => u.Commandes)
                .HasForeignKey(c => c.ExpertId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations FM1-Commande (One-to-Many)
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.FM1)
                .WithMany(f => f.Commandes)
                .HasForeignKey(c => c.FM1Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation One-to-One FM1-FM1History
            modelBuilder.Entity<FM1>()
                .HasOne(f => f.FM1History)
                .WithOne(h => h.FM1)
                .HasForeignKey<FM1History>(h => h.FM1Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation One-to-Many FM1History-Commande
            modelBuilder.Entity<FM1History>()
                .HasMany(h => h.Commandes)
                .WithOne(c => c.FM1History)
                .HasForeignKey(c => c.FM1HistoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation One-to-One Commande-Composent
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Composent)
                .WithOne(c => c.Commande)
                .HasForeignKey<Commande>(c => c.ComposentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}