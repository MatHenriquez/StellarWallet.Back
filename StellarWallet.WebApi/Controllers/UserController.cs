using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Dtos.Responses;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;
using StellarWallet.WebApi.Helpers;

namespace StellarWallet.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet()]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Result<IEnumerable<UserDto>, CustomError>>> Get()
    {
        var result = await _userService.GetAll();

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result.Error);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Result<UserDto, CustomError>>> Get(int id)
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _userService.GetById(id, jwt);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result.Error);
        }

        return Ok(result);
    }

    [HttpPost()]
    public async Task<ActionResult<Result<LoggedDto, CustomError>>> Post(UserCreationDto user)
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

    [HttpPut()]
    [Authorize]
    public async Task<ActionResult<Result<bool, CustomError>>> Put(UserUpdateDto user)
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _userService.Update(user, jwt);
        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result.Error);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<Result<bool, CustomError>>> Delete(int id)
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);
        var result = await _userService.Delete(id, jwt);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result.Error);
        }

        return Ok(result);
    }

    [HttpPost("Wallets")]
    [Authorize]
    public async Task<ActionResult<Result<bool, CustomError>>> AddWallet(
        [FromBody] AddWalletDto wallet
    )
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _userService.AddWallet(wallet, jwt);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result.Error);
        }

        return Ok(result);
    }
}
