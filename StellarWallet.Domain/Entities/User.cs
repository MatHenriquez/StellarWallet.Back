using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Domain.Entities;

public class User(string name, string lastName, string email, string password, string publicKey, string secretKey, string role = "user")
{
    [Key]
    public int Id { get; private set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must have a maximum of 50 characters and a minimun of 3")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{3,50}$", ErrorMessage = "Special characters are not allowed.")]
    public string Name { get; set; } = name;

    [Required(ErrorMessage = "Lastname is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Lastname must have a maximum of 50 characters and a minimun of 3")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{3,50}$", ErrorMessage = "Special characters are not allowed.")]
    public string LastName { get; set; } = lastName;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email")]
    public string Email { get; set; } = email;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = password;

    [Required(ErrorMessage = "Public key is required")]
    [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters.")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{56}$", ErrorMessage = "Special characters are not allowed.")]
    public string PublicKey { get; set; } = publicKey;

    [Required(ErrorMessage = "Secret key is required")]
    [StringLength(56, MinimumLength = 56, ErrorMessage = "Secret key must have a 56 characters.")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{56}$", ErrorMessage = "Special characters are not allowed.")]
    public string SecretKey { get; set; } = secretKey;

    public string Role { get; set; } = role;

    public ICollection<BlockchainAccount>? BlockchainAccounts { get; set; } = null;

    public ICollection<UserContact>? UserContacts { get; set; } = null;
}
