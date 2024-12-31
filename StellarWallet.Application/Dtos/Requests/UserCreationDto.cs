using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests;

public class UserCreationDto(string Name, string LastName, string Email, string Password, string? PublicKey, string? SecretKey)
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must have a maximum of 50 characters and a minimun of 3.")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{3,50}$", ErrorMessage = "Special characters are not allowed.")]
    public string Name { get; set; } = Name;

    [Required(ErrorMessage = "Lastname is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Lastname must have a maximum of 50 characters and a minimun of 3.")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{3,50}$", ErrorMessage = "Special characters are not allowed.")]
    public string LastName { get; set; } = LastName;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email")]
    public string Email { get; set; } = Email;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must have a maximum of 50 characters and a minimun of 8.")]
    public string Password { get; set; } = Password;

    [StringLength(56, MinimumLength = 56, ErrorMessage = "Public key must have 56 characters.")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{56}$", ErrorMessage = "Special characters are not allowed.")]
    public string? PublicKey { get; set; } = PublicKey;

    [StringLength(56, MinimumLength = 56, ErrorMessage = "Secret key must have 56 characters.")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{56}$", ErrorMessage = "Special characters are not allowed.")]
    public string? SecretKey { get; set; } = SecretKey;
}
