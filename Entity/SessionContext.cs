using Microsoft.EntityFrameworkCore;

namespace PWManager.Entity;

public class SessionContext : DbContext
{
    public DbSet<SessionEntity> Sessions { get; set; }

    public SessionEntity? GetSession(string identifier)
    {
        return Sessions.FirstOrDefault(s => s.Identifier == identifier);
    }
    public void AddSession(SessionEntity newSession)
    {
        Sessions?.Add(newSession);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source=sessions.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SessionEntity>().HasKey(s => s.Identifier);
        base.OnModelCreating(modelBuilder);
    }
}