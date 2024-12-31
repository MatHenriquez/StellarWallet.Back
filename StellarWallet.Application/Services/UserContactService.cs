using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Services;

public class UserContactService(IUserService userService, IJwtService jwtService, IMapper mapper, IAuthService authService, IUnitOfWork unitOfWork) : IUserContactService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserService _userService = userService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IAuthService _authService = authService;
    private readonly IMapper _mapper = mapper;
    private readonly CustomError _userNotFoundError = CustomError.NotFound("User not found");

    public async Task<Result<bool, CustomError>> Add(AddContactDto userContact, string jwt)
    {
        var userEmailDecoding = _jwtService.DecodeToken(jwt);

        if (!userEmailDecoding.IsSuccess)
        {
            return CustomError.InternalError(userEmailDecoding.Error.Message);
        }

        var foundUser = await _unitOfWork.User.GetBy(nameof(User.Email), userEmailDecoding.Value);

        if (foundUser is null)
        {
            return _userNotFoundError;
        }

        var isValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Email);

        if (!isValidEmail.IsSuccess)
        {
            return isValidEmail.Error;
        }

        if (foundUser.UserContacts?.Count >= 10)
        {
            return CustomError.Conflict("User has reached the maximum number of contacts");
        }

        if (foundUser.UserContacts is not null)
        {
            foreach (var contact in foundUser.UserContacts)
            {
                if (contact.Alias == userContact.Alias)
                {
                    return CustomError.Conflict("Contact already exists");
                }
            }
        }

        await _unitOfWork.UserContact.Add(new UserContact(userContact.Alias, foundUser.Id, userContact.PublicKey));

        return isValidEmail.IsSuccess;
    }

    public async Task<Result<bool, CustomError>> Delete(int id)
    {
        var userContactFound = await _unitOfWork.UserContact.GetById(id);

        if (userContactFound is null)
        {
            CustomError.NotFound("Contact not found");
        }

        await _unitOfWork.UserContact.Delete(id);

        return true;
    }

    public async Task<Result<IEnumerable<UserContactsDto>, CustomError>> GetAll(int id, string jwt)
    {
        var foundUser = await _userService.GetById(id, jwt);
        if (foundUser is null)
        {
            return _userNotFoundError;
        }

        var IsValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Value.Email);

        if (!IsValidEmail.IsSuccess)
        {
            return CustomError.Unauthorized();
        }

        var userContacts = await _unitOfWork.UserContact.GetAll(uc => uc.UserId == id);

        if (userContacts is null)
        {
            return CustomError.NotFound("Contacts not found");
        }

        return _mapper.Map<UserContactsDto[]>(userContacts);
    }

    public async Task<Result<bool, CustomError>> Update(UpdateContactDto userContact)
    {
        var foundUserContact = await _unitOfWork.UserContact.GetById(userContact.Id);

        if (foundUserContact is null)
        {
            return CustomError.NotFound("Contact not found");
        }

        if (userContact.Alias is not null)
        {
            foundUserContact.Alias = userContact.Alias;
        }

        _unitOfWork.UserContact.Update(foundUserContact);

        return true;
    }
}
