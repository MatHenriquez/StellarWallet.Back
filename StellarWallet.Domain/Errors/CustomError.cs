namespace StellarWallet.Domain.Errors
{
    public class CustomError(string? errorMessage, ErrorType errorType, int errorCode)
    {
        public string? Message { get; set; } = errorMessage;
        public ErrorType Type { get; set; } = errorType;
        public int Code { get; set; } = errorCode;
        public static CustomError NotFound(string? errorMessage = "Given parameter not found.") => new(errorMessage, ErrorType.NotFound, 404);
        public static CustomError Invalid(string? errorMessage = "Invalid parameter.") => new(errorMessage, ErrorType.Invalid, 400);
        public static CustomError Conflict(string? errorMessage = "Conflict with existing data.") => new(errorMessage, ErrorType.Conflict, 409);
        public static CustomError ExternalServiceError(string? errorMessage = "External service error.") => new(errorMessage, ErrorType.ExternalServiceError, 500);
        public static CustomError Unauthorized(string? errorMessage = "Unauthorized access.") => new(errorMessage, ErrorType.UnauthorizedError, 401);
        public static CustomError InternalError(string? errorMessage = "Internal server error.") => new(errorMessage, ErrorType.InternalError, 500);
        public static CustomError Forbidden(string? errorMessage = "Forbidden access.") => new(errorMessage, ErrorType.Forbidden, 403);
    }
}
