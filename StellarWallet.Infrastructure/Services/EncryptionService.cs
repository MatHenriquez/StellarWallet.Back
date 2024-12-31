using StellarWallet.Domain.Interfaces.Services;

namespace StellarWallet.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    public string Encrypt(string text)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(text);
    }

    public bool Verify(string text, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(text, hash);
    }
}
