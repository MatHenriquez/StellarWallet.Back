using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Interfaces
{
    public interface IUserContactService
    {
        Task<Result<IEnumerable<UserContactsDto>, CustomError>> GetAll(int id, string jwt);
        Task<Result<bool, CustomError>> Add(AddContactDto userContact, string jwt);
        Task<Result<bool, CustomError>> Update(UpdateContactDto userContact);
        Task<Result<bool, CustomError>> Delete(int id);
    }
}
