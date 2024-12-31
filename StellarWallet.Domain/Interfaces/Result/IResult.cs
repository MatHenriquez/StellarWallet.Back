namespace StellarWallet.Domain.Interfaces.Result;

public interface IResult<out TValue, out TError>
{
    TValue Value { get; }
    bool IsSuccess { get; }
    TError Error { get; }
}
