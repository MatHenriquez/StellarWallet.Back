using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StellarWallet.Domain.Entities;

public class BlockchainAccount(string PublicKey, string SecretKey, int userId)
{
    [Key]
    public int Id { get; private set; }

    [Required(ErrorMessage = "Public key is required")]
    [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters")]
    public string PublicKey { get; set; } = PublicKey;

    [Required(ErrorMessage = "Secret key is required")]
    [StringLength(56, MinimumLength = 56, ErrorMessage = "Secret key must have 56 characters")]
    public string SecretKey { get; set; } = SecretKey;

    [Required(ErrorMessage = "User id is required")]
    public int UserId { get; set; } = userId;

    [JsonIgnore]
    public User? User { get; set; } = null;

    [JsonIgnore]
    public UserContact? UserContact { get; set; } = null;
}
