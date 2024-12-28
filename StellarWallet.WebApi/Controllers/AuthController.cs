using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.Login(loginDto);

                if (!result.IsSuccess)
                {
                    return StatusCode(result.Error.Code, result);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [Authorize]
        [HttpGet("UserToken")]
        public async Task<IActionResult> UserToken()
        {

            try
            {
                string? jwt = await HttpContext.GetTokenAsync("access_token");
                if (jwt is null)
                {
                    return Unauthorized();
                }

                var result = await _authService.AuthenticateToken(jwt);

                if (!result.IsSuccess)
                {
                    return StatusCode(result.Error.Code, result);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}
