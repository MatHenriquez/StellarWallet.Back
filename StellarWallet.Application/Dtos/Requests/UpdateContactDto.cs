using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests;

public class UpdateContactDto(int id, string? alias, int? userId, int? blockchainAccountId)
{
    [Required(ErrorMessage = "User contact id is required")]
    [Range(1, int.MaxValue, ErrorMessage = "User contact id must be positive")]
    public int Id { get; set; } = id;

    [StringLength(50, MinimumLength = 3, ErrorMessage = "Alias must have a maximum of 50 characters and a minimum of 3")]
    public string? Alias { get; set; } = alias;

    [Range(1, int.MaxValue, ErrorMessage = "User id must be positive")]
    public int? UserId { get; set; } = userId;

    [Range(1, int.MaxValue, ErrorMessage = "BlockchainAccount id must be positive")]
    public int? BlockchainAccountId { get; set; } = blockchainAccountId;
}
