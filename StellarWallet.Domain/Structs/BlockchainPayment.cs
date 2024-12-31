namespace StellarWallet.Domain.Structs;

public struct BlockchainPayment
{
    public string Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Amount { get; set; }
    public string Asset { get; set; }
    public string CreatedAt { get; set; }
    public bool WasSuccessful { get; set; }
}

