using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests;

public class UserUpdateDto(int Id, string? Name, string? LastName, string? Email, string? Password, string? PublicKey, string? SecretKey)
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be a positive number")]
    public int Id { get; } = Id;

    [StringLength(50, MinimumLength = 6, ErrorMessage = "Name must have a maximum of 50 characters and a minimun of 6")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Special characters are not allowed.")]
    public string? Name { get; set; } = Name;

    [StringLength(50, MinimumLength = 6, ErrorMessage = "Lastname must have a maximum of 50 characters and a minimun of 6")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Special characters are not allowed.")]
    public string? LastName { get; set; } = LastName;

    [EmailAddress(ErrorMessage = "Invalid email")]
    public string? Email { get; set; } = Email;

    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must have a maximum of 50 characters and a minimun of 8")]
    public string? Password { get; set; } = Password;

    [StringLength(57, MinimumLength = 56, ErrorMessage = "Public key must have a maximum of 57 characters and a minimun of 56")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{1,57}$", ErrorMessage = "Special characters are not allowed.")]
    public string? PublicKey { get; set; } = PublicKey;

    [StringLength(57, MinimumLength = 56, ErrorMessage = "Secret key must have a maximum of 57 characters and a minimun of 56")]
    [RegularExpression(@"^[a-zA-Z''-'\s]{1,57}$", ErrorMessage = "Special characters are not allowed.")]
    public string? SecretKey { get; set; } = SecretKey;
}
