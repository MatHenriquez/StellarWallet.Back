namespace StellarWallet.Domain.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IUserRepository User { get; }
    IBlockchainAccountRepository BlockchainAccount { get; }
    IUserContactRepository UserContact { get; }
    Task Save();
}
