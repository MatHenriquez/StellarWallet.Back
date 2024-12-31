using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Services;

public class AuthService(IJwtService jwtService, IEncryptionService encryptionService, IUnitOfWork unitOfWork) : IAuthService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly CustomError _userNotFoundError = CustomError.NotFound("User not found");

    public async Task<Result<LoggedDto, CustomError>> Login(LoginDto loginDto)
    {
        var user = await _unitOfWork.User.GetBy(nameof(User.Email), loginDto.Email);
        if (user is null)
        {
            return _userNotFoundError;
        }

        if (!_encryptionService.Verify(loginDto.Password, user.Password))
        {
            return CustomError.Unauthorized();
        }

        var createdTokenResponse = _jwtService.CreateToken(user.Email, user.Role);

        if (!createdTokenResponse.IsSuccess)
        {
            return CustomError.InternalError(createdTokenResponse.Error.Message);
        }

        var createdToken = createdTokenResponse.Value;

        return new LoggedDto(createdTokenResponse.IsSuccess, createdToken, user.PublicKey);
    }

    public Result<bool, CustomError> AuthenticateEmail(string jwt, string email)
    {
        var jwtEmailDecoding = _jwtService.DecodeToken(jwt);

        if (!jwtEmailDecoding.IsSuccess)
        {
            return CustomError.InternalError(jwtEmailDecoding.Error.Message);
        }

        var jwtEmail = jwtEmailDecoding.Value;

        return jwtEmail.Equals(email);
    }

    public async Task<Result<bool, CustomError>> AuthenticateToken(string jwt)
    {
        var jwtEmailResponse = _jwtService.DecodeToken(jwt);

        if (!jwtEmailResponse.IsSuccess)
        {
            return CustomError.InternalError(jwtEmailResponse.Error.Message);
        }

        var email = jwtEmailResponse.Value;

        return await _unitOfWork.User.GetBy(nameof(User.Email), email) is not null;
    }
}
