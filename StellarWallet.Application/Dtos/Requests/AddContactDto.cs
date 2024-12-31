using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests;

public class AddContactDto(string alias, string publicKey)
{
    [Required(ErrorMessage = "Alias is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Alias must have a maximum of 50 characters and a minimum of 3")]
    public string Alias { get; set; } = alias;

    [Required(ErrorMessage = "Public key is required")]
    [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
    public string PublicKey { get; set; } = publicKey;
}
