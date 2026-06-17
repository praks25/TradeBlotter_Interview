using Microsoft.EntityFrameworkCore;
using TradeBlotter_Interview.Models;

namespace TradeBlotter_Interview.Data;

public class TradeBlotterDbContext(DbContextOptions<TradeBlotterDbContext> options) : DbContext(options)
{
    public DbSet<Trade> Trades => Set<Trade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trade>(entity =>
        {
            entity.Property(t => t.Symbol).HasMaxLength(20).IsRequired();
            entity.Property(t => t.Side).HasMaxLength(4).IsRequired();
            entity.HasIndex(t => t.Symbol);
        });
    }
}
