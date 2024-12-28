using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.responses.operations;
using StellarWallet.Domain.Entities;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;
using StellarWallet.Domain.Structs;
using StellarWallet.Infrastructure.Utilities;

namespace StellarWallet.Infrastructure.Services
{
    public class StellarService : IBlockchainService
    {
        private readonly string _horizonUrl;
        private readonly Network _network;
        private readonly Server _server;
        private const string _stellarNativeAsset = "native";

        public StellarService()
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";
            _horizonUrl = AppSettingsVariables.GetSettingVariable(StellarConstants.Section, StellarConstants.HorizonUrl, environmentName);
            _network = new Network(AppSettingsVariables.GetSettingVariable(StellarConstants.Section, StellarConstants.Passphrase, environmentName));
            _server = new Server(AppSettingsVariables.GetSettingVariable(StellarConstants.Section, StellarConstants.HorizonUrl, environmentName));
        }

        public Result<BlockchainAccount, CustomError> CreateAccount(int userId)
        {
            AccountKeyPair keyPair = CreateKeyPair().Value;
            return new BlockchainAccount(PublicKey: keyPair.PublicKey, SecretKey: keyPair.SecretKey, userId);
        }

        public Result<AccountKeyPair, CustomError> CreateKeyPair()
        {
            KeyPair keyPair = KeyPair.Random();

            return new AccountKeyPair
            {
                PublicKey = keyPair.AccountId,
                SecretKey = keyPair.SecretSeed
            };
        }

        public async Task<Result<bool, CustomError>> SendPayment(string sourceSecretKey, string destinationPublicKey, string amount, string assetIssuer, string assetCode, string memo)
        {
            var sourceKeypair = KeyPair.FromSecretSeed(sourceSecretKey);

            AccountResponse sourceAccountResponse;

            sourceAccountResponse = await _server.Accounts.Account(sourceKeypair.AccountId);

            var destinationKeyPair = KeyPair.FromAccountId(destinationPublicKey);

            if (sourceAccountResponse is null || destinationKeyPair is null)
            {
                return CustomError.ExternalServiceError("Invalid account");
            }

            Account sourceAccount = new(sourceKeypair.AccountId, sourceAccountResponse.SequenceNumber);

            var asset = assetIssuer == _stellarNativeAsset ? (Asset)new AssetTypeNative() : Asset.CreateNonNativeAsset(assetCode, assetIssuer);

            PaymentOperation paymentOperation = new PaymentOperation.Builder(destinationKeyPair, asset, amount).SetSourceAccount(sourceAccount.KeyPair).Build();

            Transaction transaction = new TransactionBuilder(sourceAccount)
               .AddOperation(paymentOperation)
               .AddMemo(new MemoText(memo))
               .Build();

            transaction.Sign(sourceKeypair, _network);

            SubmitTransactionResponse response = await _server.SubmitTransaction(transaction);

            if (response.IsSuccess())
            {
                return response.IsSuccess();
            }

            return CustomError.ExternalServiceError("Transaction failed");
        }

        public async Task<Result<BlockchainPayment[], CustomError>> GetPayments(string publicKey)
        {
            var payments = new List<BlockchainPayment>();

            try
            {
                var server = new Server(_horizonUrl);
                var paymentsRequestBuilder = server.Payments.ForAccount(publicKey);
                var page = await paymentsRequestBuilder.Execute();

                while (true)
                {
                    foreach (var record in page.Records.OfType<PaymentOperationResponse>())
                    {
                        payments.Add(new BlockchainPayment
                        {
                            Id = record.TransactionHash,
                            From = record.From,
                            To = record.To,
                            Amount = record.Amount,
                            Asset = record.Asset is AssetTypeNative ? _stellarNativeAsset : ((AssetTypeCreditAlphaNum)record.Asset).Code,
                            CreatedAt = record.CreatedAt.ToString(),
                            WasSuccessful = record.TransactionSuccessful
                        });
                    }

                    if (page.Records.Count == 0)
                    {
                        break;
                    }

                    page = await paymentsRequestBuilder.Cursor(page.Records[^1].PagingToken).Execute();
                }
            }
            catch (Exception e)
            {
                return CustomError.ExternalServiceError("Stellar Error " + e.Message);
            }

            return payments.ToArray();
        }

        public async Task<Result<bool, CustomError>> GetTestFunds(string publicKey)
        {
            try
            {
                var friendBotRequest = new HttpRequestMessage(HttpMethod.Get, $"{_horizonUrl}/friendbot?addr={publicKey}");
                var response = await new HttpClient().SendAsync(friendBotRequest);

                if (!response.IsSuccessStatusCode)
                {
                    return CustomError.ExternalServiceError("Test funds failed");
                }

                return Result<bool, CustomError>.Success(true);
            }
            catch (Exception e)
            {
                return CustomError.ExternalServiceError("Stellar Error " + e.Message);
            }
        }

        public async Task<Result<List<AccountBalances>, CustomError>> GetBalances(string publicKey)
        {
            try
            {
                var balances = new List<AccountBalances>();

                var account = await _server.Accounts.Account(publicKey);

                foreach (var balance in account.Balances)
                {
                    balances.Add(new AccountBalances
                    {
                        Asset = balance.Asset is AssetTypeNative ? _stellarNativeAsset : ((AssetTypeCreditAlphaNum)balance.Asset).Code,
                        Amount = balance.BalanceString,
                        Issuer = balance.Asset is AssetTypeNative ? _stellarNativeAsset : ((AssetTypeCreditAlphaNum)balance.Asset).Issuer
                    });
                }

                return balances;
            }
            catch (Exception e)
            {
                return CustomError.ExternalServiceError("Stellar Error " + e.Message);
            }
        }
    }
}
