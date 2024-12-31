using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StellarWallet.Domain.Entities;

namespace StellarWallet.Infrastructure;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<BlockchainAccount> BlockchainAccounts { get; set; }
    public DbSet<UserContact> UserContacts { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
