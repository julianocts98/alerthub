namespace AlertHub.Application.Common;

public enum ErrorKind
{
    Validation,
    NotFound,
    Unauthorized,
    Conflict,
    BadRequest,
    UnsupportedMediaType,
    Unexpected
}

public sealed record ResultError(string Code, string Message, ErrorKind Kind)
{
    public static ResultError Validation(string code, string message) => new(code, message, ErrorKind.Validation);
    public static ResultError NotFound(string code, string message) => new(code, message, ErrorKind.NotFound);
    public static ResultError Unauthorized(string code, string message) => new(code, message, ErrorKind.Unauthorized);
    public static ResultError Conflict(string code, string message) => new(code, message, ErrorKind.Conflict);
    public static ResultError BadRequest(string code, string message) => new(code, message, ErrorKind.BadRequest);
    public static ResultError UnsupportedMediaType(string code, string message) => new(code, message, ErrorKind.UnsupportedMediaType);
    public static ResultError Unexpected(string code, string message) => new(code, message, ErrorKind.Unexpected);
}

public class Result
{
    public bool IsSuccess { get; }
    public ResultError? Error { get; }

    protected Result(bool isSuccess, ResultError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure() => new(false, null);
    public static Result Failure(ResultError error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null) => Value = value;
    private Result(ResultError error) : base(false, error) { }

    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(ResultError error) => new(error);
}
