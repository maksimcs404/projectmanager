namespace Domain.Common;

public class Result<T> where T : class
{
    public T? Data;
    public bool IsSuccess { get; set; }
    public string? Message { get; set; } = string.Empty;

    private Result(T data, bool isSuccess, string message)
    {
        Data = data;
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(data, true, string.Empty);
    }

    public static Result<T> Fail(string message)
    {
        return new Result<T>(null, false, message);
    }
}