using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StellarWallet.Application.Dtos.Requests;
using StellarWallet.Application.Interfaces;
using StellarWallet.Domain.Errors;
using StellarWallet.Domain.Result;
using StellarWallet.WebApi.Helpers;

namespace StellarWallet.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    private readonly ITransactionService _transactionService = transactionService;

    [HttpPost("AccountCreation")]
    public async Task<ActionResult<Result<bool, CustomError>>> CreateAccount()
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _transactionService.CreateAccount(jwt);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [HttpPost("Payments")]
    public async Task<ActionResult<Result<bool, CustomError>>> SendPayment(
        [FromBody] SendPaymentDto sendPaymentDto
    )
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var paymentResult = await _transactionService.SendPayment(sendPaymentDto, jwt);

        if (!paymentResult.IsSuccess)
        {
            return StatusCode(paymentResult.Error.Code, paymentResult.Error.Message);
        }

        return Ok();
    }

    [HttpGet("Payments")]
    public async Task<ActionResult<Result<bool, CustomError>>> GetPayments(
        [FromQuery] int pageNumber,
        int pageSize
    )
    {
        var jwt = await JwtTokenHelper.GetFromContextAsync(HttpContext);

        var result = await _transactionService.GetTransaction(jwt, pageNumber, pageSize);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [HttpPost("TestFund")]
    public async Task<ActionResult<Result<bool, CustomError>>> GetTestFunds(
        [FromBody] GetTestFundsDto getTestFundsDto
    )
    {
        var result = await _transactionService.GetTestFunds(getTestFundsDto.PublicKey);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }

    [HttpGet("Balances")]
    public async Task<ActionResult<Result<bool, CustomError>>> GetBalances(
        [FromQuery] GetBalancesDto getBalancesDto
    )
    {
        var result = await _transactionService.GetBalances(getBalancesDto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.Error.Code, result);
        }

        return Ok(result);
    }
}
