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
        public DbSet<FM1History> FM1Histories { get; set; } 
        public DbSet<ExcelFm1> ExcelFm1s { get; set; }
        public DbSet<ExcelComposent> ExcelComposents { get; set; }

        public QueryDbContext(DbContextOptions<QueryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FM1>().ToTable("FM1s");
            modelBuilder.Entity<Composent>().ToTable("Composents");
            modelBuilder.Entity<Commande>().ToTable("Commandes");
            modelBuilder.Entity<FM1History>().ToTable("FM1Histories"); // Ajout de la table FM1Histories
            modelBuilder.Entity<ExcelFm1>().ToTable("ExcelFm1s");
            modelBuilder.Entity<ExcelComposent>().ToTable("ExcelComposents");
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

            // Relation One-to-One entre FM1 et FM1History
            modelBuilder.Entity<FM1>()
                .HasOne(f => f.FM1History)
                .WithOne(h => h.FM1)
                .HasForeignKey<FM1History>(h => h.FM1Id)
                .OnDelete(DeleteBehavior.Cascade); // ou DeleteBehavior.SetNull si vous voulez autoriser FM1 sans FM1History


            // Relation One-to-Many entre FM1History et Commande
            modelBuilder.Entity<FM1History>()
                .HasMany(h => h.Commandes)
                .WithOne(c => c.FM1History)
                .HasForeignKey(c => c.FM1HistoryId)
                .OnDelete(DeleteBehavior.SetNull); //Si on supprime l'historique, la commande peut exister (FM1HistoryId = null)

           modelBuilder.Entity<Composent>()
    .HasOne(c => c.Commande)
    .WithOne(co => co.Composent)
    .HasForeignKey<Composent>(c => c.CommandeId)  // Utiliser la clé étrangère explicite
    .OnDelete(DeleteBehavior.SetNull); // Ajustez le comportement de suppression selon vos besoins
        }
    }
}