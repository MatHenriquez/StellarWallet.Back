using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Domain.Interfaces.Services
{
    public interface IJwtService
    {
        Result<string, CustomError> CreateToken(string email, string role);
        Result<string, CustomError> DecodeToken(string token);
    }
}
