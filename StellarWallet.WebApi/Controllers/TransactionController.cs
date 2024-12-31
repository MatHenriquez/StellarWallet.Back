using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;

namespace StellarWallet.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TransactionController(ITransactionService transactionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;
    private readonly string _accessToken = "access_token";

    [HttpPost("AccountCreation")]
    public async Task<IActionResult> CreateAccount()
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync(_accessToken);
            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _transactionService.CreateAccount(jwt);

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

    [HttpPost("Payment")]
    public async Task<IActionResult> SendPayment([FromBody] SendPaymentDto sendPaymentDto)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync(_accessToken);
            if (jwt is null)
            {
                return Unauthorized();
            }

            var paymentResult = await _transactionService.SendPayment(sendPaymentDto, jwt);

            if (!paymentResult.IsSuccess)
            {
                return StatusCode(paymentResult.Error.Code, paymentResult.Error.Message);
            }
            return Ok();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "There was an error processing the payment.");
        }
    }

    [HttpGet("Payment")]
    public async Task<IActionResult> GetPayments([FromQuery] int pageNumber, int pageSize)
    {
        try
        {
            var jwt = await HttpContext.GetTokenAsync(_accessToken);
            if (jwt is null)
            {
                return Unauthorized();
            }

            var result = await _transactionService.GetTransaction(jwt, pageNumber, pageSize);

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

    [HttpPost("TestFund")]
    public async Task<IActionResult> GetTestFunds([FromBody] GetTestFundsDto getTestFundsDto)
    {
        var jwt = await HttpContext.GetTokenAsync(_accessToken);
        if (jwt is null)
        { return Unauthorized(); }

        var result = await _transactionService.GetTestFunds(getTestFundsDto.PublicKey);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [HttpGet("Balance")]
    public async Task<IActionResult> GetBalances([FromQuery] GetBalancesDto getBalancesDto)
    {
        var jwt = await HttpContext.GetTokenAsync(_accessToken);
        if (jwt is null)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _transactionService.GetBalances(getBalancesDto);

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

