using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;
using StellarWallet.WebApi.Helpers;

namespace StellarWallet.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("Login")]
    public async Task<ActionResult<Result<bool, CustomError>>> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _authService.Login(loginDto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("UserToken")]
    public async Task<ActionResult<Result<bool, CustomError>>> UserToken()
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _authService.AuthenticateToken(jwt);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }
}
