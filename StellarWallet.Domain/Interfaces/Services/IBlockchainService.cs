using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Domain.Interfaces.Services;

public interface IBlockchainService
{
    public Result<BlockchainAccount, CustomError> CreateAccount(int userId);
    public Result<AccountKeyPair, CustomError> CreateKeyPair();
    public Task<Result<bool, CustomError>> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount, string assetIssuer, string assetCode, string memo);
    public Task<Result<BlockchainPayment[], CustomError>> GetPayments(string publicKey);
    public Task<Result<bool, CustomError>> GetTestFunds(string publicKey);
    public Task<Result<List<AccountBalances>, CustomError>> GetBalances(string publicKey);
}
