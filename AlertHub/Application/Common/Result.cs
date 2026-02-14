namespace AlertHub.Application.Common;

public sealed record ResultError(string Code, string Message);

public class Result {
    public bool IsSuccess { get; }
    public ResultError? Error { get; }
    
    protected  Result(bool isSuccess, ResultError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Success() => new(true, null);
    public static Result Failure() => new(false, null);
}

public class Result<T> : Result {
    public T? Value { get; }

    private Result(T value) : base(true, null) => Value = value;
    private Result(ResultError error) : base(false, error) {}

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(ResultError error) => new(error);
}