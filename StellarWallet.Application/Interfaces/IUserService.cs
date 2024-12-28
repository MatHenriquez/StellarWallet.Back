using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();
        Task<Result<UserDto, CustomError>> GetById(int id, string jwt);
        Task<Result<LoggedDto, CustomError>> Add(UserCreationDto user);
        Task<Result<bool, CustomError>> Update(UserUpdateDto user, string jwt);
        Task<Result<bool, CustomError>> Delete(int id, string jwt);
        Task<Result<bool, CustomError>> AddWallet(AddWalletDto wallet, string jwt);
    }
}
