using StellarWallet.Domain.Interfaces.Persistence;

namespace StellarWallet.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;
    public IUserRepository User { get; }
    public IBlockchainAccountRepository BlockchainAccount { get; }
    public IUserContactRepository UserContact { get; }

    public UnitOfWork(DatabaseContext context)
    {
        _context = context;
        User = new UserRepository(_context);
        BlockchainAccount = new BlockchainAccountRepository(_context);
        UserContact = new UserContactRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}
