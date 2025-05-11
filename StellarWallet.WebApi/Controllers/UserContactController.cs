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
[Authorize]
[Route("[controller]")]
public class UserContactsController(IUserContactService userContactService) : ControllerBase
{
    private readonly IUserContactService _userContactService = userContactService;

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<IEnumerable<UserContactsDto>, CustomError>>> GetAll(
        int id
    )
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _userContactService.GetAll(id, jwt);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool, CustomError>>> Delete(int id)
    {
        var result = await _userContactService.Delete(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [HttpPost()]
    public async Task<ActionResult<Result<bool, CustomError>>> Post(AddContactDto userContact)
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _userContactService.Add(userContact, jwt);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [HttpPut()]
    public async Task<ActionResult<Result<bool, CustomError>>> Put(UpdateContactDto userContact)
    {
        var result = await _userContactService.Update(userContact);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }
}
