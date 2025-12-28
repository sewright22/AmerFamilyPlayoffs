using Microsoft.EntityFrameworkCore;
using NflPlayoffPool.Data.Models;

namespace NflPlayoffPool.TestCommon;

/// <summary>
/// Test-specific DbContext that excludes problematic entities for unit testing
/// </summary>
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Bracket> Brackets { get; set; }
    public DbSet<Season> Seasons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entities for in-memory testing
        modelBuilder.Entity<Season>().HasKey(s => s.Id);
        modelBuilder.Entity<Bracket>().HasKey(b => b.Id);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        
        // Configure relationships
        modelBuilder.Entity<Bracket>()
            .HasMany(b => b.Picks)
            .WithOne()
            .HasForeignKey("BracketId");
            
        modelBuilder.Entity<Season>()
            .HasMany(s => s.Teams)
            .WithOne()
            .HasForeignKey("SeasonId");
    }
}