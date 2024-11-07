using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public  class BankDatabase: DbContext
{
    public BankDatabase(DbContextOptions<BankDatabase> options)
        : base(options) { }

public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
}