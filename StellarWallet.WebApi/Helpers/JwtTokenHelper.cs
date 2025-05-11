using Microsoft.AspNetCore.Authentication;

namespace StellarWallet.WebApi.Helpers;

public static class JwtTokenHelper
{
    private const string ACCESS_TOKEN = "access_token";

    public static async Task<string> GetFromContextAsync(HttpContext httpContext)
    {
        var token = await httpContext.GetTokenAsync(ACCESS_TOKEN);

        if (token is null)
        {
            throw new ArgumentException("No access token found");
        }

        return token;
    }
}
