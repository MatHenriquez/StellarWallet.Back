namespace StellarWallet.Domain.Structs;

public struct AccountBalances
{
    public string Asset { get; set; }
    public string Amount { get; set; }
    public string Issuer { get; set; }
}
