using Microsoft.AspNetCore.Diagnostics;
using StellarWallet.Domain.Errors;
using System.Text.Json;
using StellarWallet.Domain.Result;

namespace StellarWallet.WebApi.Utilities;

internal sealed class GlobalExceptionHandler() : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var error = CustomError.InternalError(exception.Message);
        var response = Result<string, CustomError>.Failure(error);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await httpContext.Response.WriteAsJsonAsync(response, options, cancellationToken);

        return true;
    }
}
