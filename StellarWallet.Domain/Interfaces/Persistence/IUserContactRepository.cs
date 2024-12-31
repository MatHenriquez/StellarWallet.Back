using StellarWallet.Domain.Entities;

namespace StellarWallet.Domain.Interfaces.Persistence;

public interface IUserContactRepository : IRepository<UserContact>
{
    Task Delete(int id);
}
