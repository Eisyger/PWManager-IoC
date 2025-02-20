using Microsoft.EntityFrameworkCore;
using PWManager.Context;

namespace PWManager.Interfaces;

public interface IAccountContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public AccountEntity? CurrentAccEntity { get; set; }
}