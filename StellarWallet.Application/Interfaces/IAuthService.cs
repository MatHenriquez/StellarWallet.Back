using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoggedDto, CustomError>> Login(LoginDto loginDto);
        Result<bool, CustomError> AuthenticateEmail(string jwt, string email);
        Task<Result<bool, CustomError>> AuthenticateToken(string jwt);
    }
}
