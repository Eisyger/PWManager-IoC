using Microsoft.EntityFrameworkCore;
using PWManager.Config;

namespace PWManager.Entity;

public class AccountContext : DbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public AccountEntity? CurrentAccEntity { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>().HasKey(s => s.User);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = Configuration.GetConfig();
        
        var section = config.GetSection("Database");
        var basePath = section.GetSection("ConnectionStringWithoutFilename").Value;
        var filename = section.GetSection("Filename").Value;

        if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(filename))
            throw new NullReferenceException("Der Connection String zur Database ist leer, null oder Whitespace. Prüfe deine Configuration, bzw. die appsettings.json");
        var dbPath = Path.Combine(basePath, filename);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}