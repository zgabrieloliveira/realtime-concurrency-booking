using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence;

public class TicketDbContext : DbContext
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
    {
    }
    
    public DbSet<Seat> Seats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Row).IsRequired().HasMaxLength(5);
            entity.Property(e => e.Number).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.HasIndex(e => new { e.Row, e.Number }).IsUnique();
        });
        base.OnModelCreating(modelBuilder);
    }
}