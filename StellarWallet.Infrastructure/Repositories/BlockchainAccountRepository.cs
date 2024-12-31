using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Persistence;

namespace StellarWallet.Infrastructure.Repositories;

public class BlockchainAccountRepository : Repository<BlockchainAccount>, IBlockchainAccountRepository
{
    private readonly DatabaseContext _context;

    public BlockchainAccountRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }
}
