using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Interfaces.Persistence;

namespace StellarWallet.Infrastructure.Repositories;

public class UserContactRepository : Repository<UserContact>, IUserContactRepository
{
    private readonly DatabaseContext _context;

    public UserContactRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }

    public async Task Delete(int id)
    {
        var foundUserContact = await GetById(id) ?? throw new KeyNotFoundException("User contact not found");
        _context.UserContacts.Remove(foundUserContact);
        await _context.SaveChangesAsync();
    }
}
