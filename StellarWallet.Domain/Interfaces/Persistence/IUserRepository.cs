using StellarWallet.Domain.Entities;

namespace StellarWallet.Domain.Interfaces.Persistence;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetBy(string criteria, string value);
    Task Delete(int id);
}
