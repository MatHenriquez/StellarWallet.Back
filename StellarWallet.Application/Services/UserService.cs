using AutoMapper;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Services
{
    public class UserService(IJwtService jwtService, IEncryptionService encryptionService, IMapper mapper, IBlockchainService stellarService, IBlockchainAccountRepository blockchainAccountRepository, IAuthService authService, IUnitOfWork unitOfWork
        ) : IUserService
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IEncryptionService _encryptionService = encryptionService;
        private readonly IBlockchainService _stellarService = stellarService;
        private readonly IBlockchainAccountRepository _blockchainAccountRepository = blockchainAccountRepository;
        private readonly IAuthService _authService = authService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly CustomError _userNotFoundError = CustomError.NotFound("User not found");

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            IEnumerable<User> users = await _unitOfWork.User.GetAll();
            return _mapper.Map<UserDto[]>(users);
        }

        public async Task<Result<UserDto, CustomError>> GetById(int id, string jwt)
        {
            User? foundUser = await _unitOfWork.User.GetById(id);

            if (foundUser is null)
            {
                return _userNotFoundError;
            }

            var IsValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Email);

            if (!IsValidEmail.IsSuccess)
            {
                return CustomError.Forbidden();
            }

            var mappedUser = _mapper.Map<UserDto>(foundUser);

            return mappedUser;
        }

        public async Task<Result<LoggedDto, CustomError>> Add(UserCreationDto user)
        {
            User? foundUser = await _unitOfWork.User.GetBy(nameof(User.Email), user.Email);
            if (foundUser is not null)
            {
                return CustomError.Conflict("User already exists.");
            }

            user.Password = _encryptionService.Encrypt(user.Password);

            AccountKeyPair account = _stellarService.CreateKeyPair().Value;
            user.PublicKey = account.PublicKey;
            user.SecretKey = account.SecretKey;

            await _unitOfWork.User.Add(_mapper.Map<User>(user));

            return new LoggedDto(true, null, user.PublicKey);
        }

        public async Task<Result<bool, CustomError>> Update(UserUpdateDto user, string jwt)
        {
            if (user.Password is not null)
            {
                user.Password = _encryptionService.Encrypt(user.Password);
            }

            User? foundUser = await _unitOfWork.User.GetById(user.Id);
            if (foundUser is null)
            {
                return _userNotFoundError;
            }

            var IsValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Email);

            if (!IsValidEmail.IsSuccess)
            {
                return CustomError.Forbidden();
            }

            _unitOfWork.User.Update(_mapper.Map<User>(user));

            return IsValidEmail.IsSuccess;
        }

        public async Task<Result<bool, CustomError>> Delete(int id, string jwt)
        {
            User? foundUser = await _unitOfWork.User.GetById(id);
            if (foundUser is null)
            {
                return _userNotFoundError;
            }

            var IsValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Email);

            if (!IsValidEmail.IsSuccess)
            {
                return IsValidEmail.Error;
            }

            await _unitOfWork.User.Delete(id);

            return IsValidEmail.IsSuccess;
        }

        public async Task<Result<bool, CustomError>> AddWallet(AddWalletDto wallet, string jwt)
        {
            try
            {
                var email = _jwtService.DecodeToken(jwt);
                User? foundUser = await _unitOfWork.User.GetBy(nameof(User.Email), email.Value);
                if (foundUser is null)
                {
                    return _userNotFoundError;
                }

                var IsValidEmail = _authService.AuthenticateEmail(jwt, foundUser.Email);

                if (!IsValidEmail.IsSuccess)
                {
                    return IsValidEmail.Error;
                }

                if (foundUser.BlockchainAccounts is not null)
                {
                    foreach (BlockchainAccount account in foundUser.BlockchainAccounts)
                    {
                        if (account.PublicKey == wallet.PublicKey)
                        {
                            return CustomError.Conflict("Wallet already exists.");
                        }
                    }

                    if (foundUser.BlockchainAccounts.Count >= 5)
                    {
                        return CustomError.Conflict("User already has 5 wallets.");
                    }
                }

                BlockchainAccount newAccount = new(wallet.PublicKey, wallet.SecretKey, foundUser.Id)
                {
                    User = foundUser
                };

                await _blockchainAccountRepository.Add(newAccount);
                _unitOfWork.User.Update(foundUser);

                return true;
            }
            catch (Exception e)
            {
                return CustomError.InternalError(e.Message);
            }
        }
    }
}
