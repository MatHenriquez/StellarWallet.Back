namespace StellarWallet.Domain.Interfaces.Services;

public interface IEncryptionService
{
    string Encrypt(string text);
    bool Verify(string text, string hash);
}
