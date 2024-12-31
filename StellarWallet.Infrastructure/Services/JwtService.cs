using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Interfaces.Services;
using StellarWallet.Domain.Result;

namespace StellarWallet.Infrastructure.Services;

public class JwtService(IConfiguration config) : IJwtService
{
    private readonly string? secretKey = config.GetSection("Jwt").GetSection("Key").Value;
    private readonly string? issuer = config.GetSection("Jwt").GetSection("Issuer").Value;
    private readonly string? audience = config.GetSection("Jwt").GetSection("Audience").Value;

    public Result<string, CustomError> CreateToken(string email, string role)
    {
        if (secretKey is null)
        {
            return CustomError.NotFound("No secret key found.");
        }

        var keyBytes = Encoding.ASCII.GetBytes(secretKey);
        var claims = new ClaimsIdentity();

        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
        claims.AddClaim(new Claim(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddYears(100),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
            Issuer = issuer,
            Audience = audience
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(tokenConfig);
    }

    public Result<string, CustomError> DecodeToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        if (secretKey is null)
        {
            return CustomError.NotFound("No secret key found.");
        }

        var keyBytes = Encoding.ASCII.GetBytes(secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

        if (claimsPrincipal is null)
        {
            return CustomError.NotFound("No claims found.");
        }

        var allClaims = claimsPrincipal.Claims.ToList();

        var emailClaim = allClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var roleClaim = allClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        if (emailClaim is null || roleClaim is null)
        {
            return CustomError.NotFound("Email or role claim not found.");
        }

        return emailClaim.Value;
    }
}
