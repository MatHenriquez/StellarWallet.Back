using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;

namespace StellarWallet.WebApi.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet()]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var result = await _userService.GetAll();

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync("access_token");

            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _userService.GetById(id, jwt);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
        }
    }

    [HttpPost()]
    public async Task<ActionResult<Result<LoggedDto, CustomError>>> Post(UserCreationDto user)
    {
        try
        {
            var result = await _userService.Add(user);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(result.Error.Code, result.Error);
            }
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {e.Message}");
        }
    }

    [HttpPut()]
    [Authorize]
    public async Task<IActionResult> Put(UserUpdateDto user)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _userService.Update(user, jwt);
            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
        }

    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _userService.Delete(id, jwt);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {e.Message}");
        }
    }

    [HttpPost("Wallet")]
    [Authorize]
    public async Task<IActionResult> AddWallet([FromBody] AddWalletDto wallet)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _userService.AddWallet(wallet, jwt);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result.Error);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {e.Message}");
        }
    }
}
