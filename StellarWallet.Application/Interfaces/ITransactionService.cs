using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Result;
using StellarWallet.Domain.Result;

namespace StellarWallet.Application.Interfaces
{
    public interface ITransactionService
    {
        public Task<Result<BlockchainAccount, CustomError>> CreateAccount(string jwt);
        public Task<Result<bool, CustomError>> SendPayment(SendPaymentDto sendPaymentDto, string jwt);
        public Task<Result<TransactionsDto, CustomError>> GetTransaction(string jwt, int pageNumber, int pageSize);
        public Task<Result<bool, CustomError>> GetTestFunds(string publicKey);
        public Task<Result<FoundBalancesDto, CustomError>> GetBalances(GetBalancesDto getBalancesDto);
    }
}
