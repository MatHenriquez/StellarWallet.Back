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
[Authorize]
[Route("[controller]")]
public class UserContactsController(IUserContactService userContactService) : ControllerBase
{
    private readonly IUserContactService _userContactService = userContactService;

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<IEnumerable<UserContactsDto>, CustomError>>> GetAll(int id)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _userContactService.GetAll(id, jwt);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {e.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool, CustomError>>> Delete(int id)
    {
        try
        {
            var result = await _userContactService.Delete(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {e.Message}");
        }
    }

    [HttpPost()]
    public async Task<ActionResult<Result<bool, CustomError>>> Post(AddContactDto userContact)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync("access_token");
            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _userContactService.Add(userContact, jwt);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    [HttpPut()]
    public async Task<ActionResult<Result<bool, CustomError>>> Put(UpdateContactDto userContact)
    {
        try
        {
            var result = await _userContactService.Update(userContact);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Error.Code, result);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {e.Message}");
        }
    }
}
