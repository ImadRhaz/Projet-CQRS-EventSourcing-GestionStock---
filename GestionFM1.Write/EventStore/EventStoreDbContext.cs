using Microsoft.EntityFrameworkCore;

namespace GestionFM1.Write.EventStore;

public class EventStoreDbContext : DbContext
{
    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
    {
    }

    public DbSet<EventEntity> Events { get; set; } = null!;  // <-- IMPORTANT: Use null! to suppress warnings

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventEntity>()
            .HasKey(e => e.Id);
    }
}