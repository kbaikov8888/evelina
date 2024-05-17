using Microsoft.EntityFrameworkCore;

namespace PortfolioImpl.Database;

internal sealed class PortfolioContext : DbContext
{
    public DbSet<Thing> Things => Set<Thing>();

    private readonly string _path;

    public PortfolioContext(string path)
    {
        _path = path;

        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_path}");
    }
}