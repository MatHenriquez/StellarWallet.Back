namespace StellarWallet.Application.Dtos.Responses;

public class LoggedDto(bool success, string? token, string? publicKey)
{
    public bool Success { get; set; } = success;
    public string? Token { get; set; } = token;
    public string? PublicKey { get; set; } = publicKey;
}
