using StellarWallet.Domain.Interfaces.Result;
using System.Text.Json.Serialization;

namespace StellarWallet.Domain.Result
{
    public class Result<TValue, TError> : IResult<TValue, TError>
    {
        public bool IsSuccess { get; private set; }
        public TValue Value { get; private set; }
        public TError Error { get; private set; }

        [JsonConstructor]
        public Result(bool isSuccess, TValue? value, TError? error)
        {
            IsSuccess = isSuccess;
            Value = value ?? default!;
            Error = error ?? default!;
        }

        public Result() { }

        private Result(TValue value)
        {
            IsSuccess = true;
            Value = value;
            Error = default!;
        }

        private Result(TError error)
        {
            IsSuccess = false;
            Error = error;
            Value = default!;
        }

        public static Result<TValue, TError> Success(TValue value) => new(value);
        public static Result<TValue, TError> Failure(TError error) => new(error);

        public static implicit operator Result<TValue, TError>(TValue success) => Success(success);
        public static implicit operator Result<TValue, TError>(TError error) => Failure(error);
    }
}