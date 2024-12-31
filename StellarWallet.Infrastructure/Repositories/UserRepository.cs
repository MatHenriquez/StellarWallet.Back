using Microsoft.EntityFrameworkCore;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Persistence;

namespace StellarWallet.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetBy(string paramName, string paramValue)
    {
        var propertyInfo = typeof(User).GetProperty(paramName) ?? throw new ArgumentException($"Invalid property: '{paramName}'.");
        var query = _context.Users.Where(u => EF.Property<string>(u, propertyInfo.Name) == paramValue).Include(u => u.BlockchainAccounts).Include(u => u.UserContacts);

        try
        {
            return await query.FirstAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task Delete(int id)
    {
        var foundUser = await GetById(id) ?? throw new Exception("User not found");
        _context.Users.Remove(foundUser);
        await _context.SaveChangesAsync();
    }
}
