using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PWManager.Interfaces;

namespace PWManager.Context;

public class AccountContext(DbContextOptions<AccountContext> options, IConfiguration config) : DbContext(options), IAccountContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public AccountEntity? CurrentAccEntity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>().HasKey(s => s.User);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var section = config.GetSection("Database");
        var basePath = section.GetSection("ConnectionStringWithoutFilename").Value;
        var filename = section.GetSection("Filename").Value;

        if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(filename))
            throw new NullReferenceException("Der Connection String zur Database ist leer, null oder Whitespace. Prüfe deine Configuration, bzw. die appsettings.json");
        var dbPath = Path.Combine(basePath, filename);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}