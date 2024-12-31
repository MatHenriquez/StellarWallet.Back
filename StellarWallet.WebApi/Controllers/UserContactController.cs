using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserContactController(IUserContactService userContactService) : ControllerBase
{
    private readonly IUserContactService _userContactService = userContactService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAll(int id)
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
    public async Task<IActionResult> Delete(int id)
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
    public async Task<IActionResult> Post(AddContactDto userContact)
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
    public async Task<IActionResult> Put(UpdateContactDto userContact)
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
