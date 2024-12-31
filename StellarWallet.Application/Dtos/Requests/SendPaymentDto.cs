using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests;

public class SendPaymentDto(string destinationPublicKey, decimal amount, string? assetIssuer, string? assetCode, string memo)
{
    [Required(ErrorMessage = "Public key is required")]
    [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
    public string DestinationPublicKey { get; set; } = destinationPublicKey;

    [MaxLength(56, ErrorMessage = "Asset issuer must have 56 characters")]
    public string AssetIssuer { get; set; } = assetIssuer ?? "native";

    [StringLength(12, MinimumLength = 1, ErrorMessage = "Asset code must have between 1 and 12 characters")]
    public string AssetCode { get; set; } = assetCode ?? "XLM";

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.0001, double.MaxValue, ErrorMessage = "Invalid amount")]
    public decimal Amount { get; set; } = amount;

    [StringLength(28, MinimumLength = 0, ErrorMessage = "Memo must have between 0 and 28 characters")]
    public string? Memo { get; set; } = memo;
}
