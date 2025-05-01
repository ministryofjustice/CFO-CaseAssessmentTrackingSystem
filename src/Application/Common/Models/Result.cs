namespace Cfo.Cats.Application.Common.Models;

public class Result : IResult
{
    internal Result()
    {
        Errors = new string[] { };
    }

    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public string ErrorMessage => string.Join(", ", Errors ?? new string[] { });

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(params string[] errors)
    {
        return new Result(false, errors);
    }

}

public class Result<T> : Result, IResult<T>
{
    public T? Data { get; set; }

    public new static Result<T> Failure(params string[] errors)
    {
        return new Result<T> { Succeeded = false, Errors = errors.ToArray() };
    }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static Result<T> NotFound()
    {
        return new NotFoundResult<T>();
    }

    public static implicit operator Result<T>(T data) => Success(data);

    public static implicit operator T(Result<T> result) => result.Data!;
}

public class NotFoundResult<T> : Result<T>
{
    public NotFoundResult()
    {
        Succeeded = false;
        Data = default(T);
        Errors = ["No data found"];
    }
}
    