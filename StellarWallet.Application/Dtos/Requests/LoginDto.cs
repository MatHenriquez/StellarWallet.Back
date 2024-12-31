using System.ComponentModel.DataAnnotations;

namespace StellarWallet.Application.Dtos.Requests;

public class LoginDto(string email, string password)
{
    [EmailAddress(ErrorMessage = "Must be a valid email address"), Required(ErrorMessage = "Email must be provided")]
    public string Email { get; set; } = email;

    [Required(ErrorMessage = "Password must be provided")]
    [Length(8, 50, ErrorMessage = "Password must have between 8 and 50 characters")]
    public string Password { get; set; } = password;
}
