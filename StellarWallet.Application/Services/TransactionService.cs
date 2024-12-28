using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Application.Utilities;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Persistence;
using StellarWallet.Domain.Interfaces.Result;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Application.Services
{
    public class TransactionService(IBlockchainService blockchainService, IJwtService jwtService, IAuthService authService, IUnitOfWork unitOfWork) : ITransactionService
    {
        private readonly IBlockchainService _blockchainService = blockchainService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthService _authService = authService;
        private readonly CustomError _userNotFoundError = CustomError.NotFound("User not found");

        public async Task<Result<BlockchainAccount, CustomError>> CreateAccount(string jwt)
        {
            var userEmail = _jwtService.DecodeToken(jwt);
            if (!userEmail.IsSuccess)
            {
                return CustomError.Unauthorized();
            }

            User? user = await _unitOfWork.User.GetBy(nameof(User.Email), userEmail.Value);
            if (user is null)
            {
                return _userNotFoundError;
            }

            return _blockchainService.CreateAccount(user.Id);
        }

        public async Task<Result<bool, CustomError>> SendPayment(SendPaymentDto sendPaymentDto, string jwt)
        {
            var userEmail = _jwtService.DecodeToken(jwt);
            User? user = await _unitOfWork.User.GetBy(nameof(User.Email), userEmail.Value);

            if (user is null)
            {
                return _userNotFoundError;
            }

            var isAnAuthorizedUser = _authService.AuthenticateEmail(jwt, userEmail.Value);
            if (!isAnAuthorizedUser.IsSuccess)
            {
                return CustomError.Unauthorized();
            }

            var transactionResponse = await _blockchainService.SendPayment(user.SecretKey, sendPaymentDto.DestinationPublicKey, sendPaymentDto.Amount.ToString(), sendPaymentDto.AssetIssuer, sendPaymentDto.AssetCode, sendPaymentDto.Memo ?? String.Empty);

            bool transactionCompleted = transactionResponse.IsSuccess;

            if (!transactionCompleted)
            {
                return CustomError.ExternalServiceError(transactionResponse?.Error?.Message);
            }

            return transactionCompleted;
        }

        public async Task<Result<TransactionsDto, CustomError>> GetTransaction(string jwt, int pageNumber, int pageSize)
        {
            var userEmailDecoding = _jwtService.DecodeToken(jwt);

            if (!userEmailDecoding.IsSuccess)
            {
                return Result<TransactionsDto, CustomError>.Failure(CustomError.Unauthorized());
            }

            User? user = await _unitOfWork.User.GetBy(nameof(User.Email), userEmailDecoding.Value);

            if (user is null)
            {
                return _userNotFoundError;
            }

            var isAnAuthorizedUser = _authService.AuthenticateEmail(jwt, userEmailDecoding.Value);
            if (!isAnAuthorizedUser.IsSuccess)
            {
                CustomError.Unauthorized();
            }

            var getPaymentsResponse = await _blockchainService.GetPayments(user.PublicKey);

            if (!getPaymentsResponse.IsSuccess)
            {
                return CustomError.ExternalServiceError(getPaymentsResponse.Error.Message);
            }

            BlockchainPayment[] allPayments = getPaymentsResponse.Value;

            List<BlockchainPayment> paginatedPayments = allPayments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = Paginate.GetTotalPages(allPayments.Length, pageSize);

            TransactionsDto paginatedPaymentsDto = new(paginatedPayments, totalPages);

            return Result<TransactionsDto, CustomError>.Success(paginatedPaymentsDto);
        }

        public async Task<Result<bool, CustomError>> GetTestFunds(string publicKey)
        {
            var getTestFundsResponse = await _blockchainService.GetTestFunds(publicKey);
            if (!getTestFundsResponse.IsSuccess)
            {
                return CustomError.ExternalServiceError(getTestFundsResponse.Error.Message);
            }

            return getTestFundsResponse.Value;
        }

        public async Task<Result<FoundBalancesDto, CustomError>> GetBalances(GetBalancesDto getBalancesDto)
        {
            var getBalancesResponse = await _blockchainService.GetBalances(getBalancesDto.PublicKey);

            if (!getBalancesResponse.IsSuccess)
            {
                return CustomError.ExternalServiceError(getBalancesResponse.Error.Message);
            }

            List<AccountBalances> balances = getBalancesResponse.Value;

            if (getBalancesDto.FilterZeroBalances)
            {
                balances = balances.Where(balance => balance.Amount != "0.0000000").ToList();
            }

            int totalPages = Paginate.GetTotalPages(balances.Count, getBalancesDto.PageSize);
            var paginatedBalances = Paginate.PaginateQuery<AccountBalances>(balances, getBalancesDto.PageNumber, getBalancesDto.PageSize).ToList();

            return new FoundBalancesDto(paginatedBalances, totalPages);
        }
    }
}
