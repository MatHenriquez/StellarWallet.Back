using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;

namespace StellarWallet.Domain.Interfaces.Services;

public interface IBlockchainService
{
    Result<BlockchainAccount, CustomError> CreateAccount(int userId);
    Result<AccountKeyPair, CustomError> CreateKeyPair();
    Task<Result<bool, CustomError>> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount, string assetIssuer, string assetCode, string memo);
    Task<Result<BlockchainPayment[], CustomError>> GetPayments(string publicKey);
    Task<Result<bool, CustomError>> GetTestFunds(string publicKey);
    Task<Result<List<AccountBalances>, CustomError>> GetBalances(string publicKey);
}
