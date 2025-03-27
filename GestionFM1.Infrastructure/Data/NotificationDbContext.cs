using Microsoft.EntityFrameworkCore;
using GestionFM1.Infrastructure.Notification;

namespace GestionFM1.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options) { }

    protected NotificationDbContext() // Ajout du constructeur sans paramètres
    {
    }

    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(n => n.UserId);
            entity.HasIndex(n => n.IsRead);
            entity.HasIndex(n => n.CreatedAt);
            entity.Property(n => n.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
}