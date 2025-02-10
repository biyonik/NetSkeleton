using System.Text.Json.Serialization;

namespace Application.Common.Results;

/// <summary>
/// Base result sınıfı
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public string Message { get; }

    protected Result(bool isSuccess, Error error, string message)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }

    public static Result Success(string message = "") => new(true, Error.None, message);
    public static Result Failure(Error error) => new(false, error, error.Message);
    public static Result Failure(string message) => new(false, Error.Custom(message), message);

    public static Result<T> Success<T>(T value, string message = "") => Result<T>.Success(value, message);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
    public static Result<T> Failure<T>(string message) => Result<T>.Failure(message);
}

/// <summary>
/// Generic result sınıfı
/// </summary>
[method: JsonConstructor]
public class Result<T>(T? value, bool isSuccess, Error error, string message)
    : Result(isSuccess, error, message)
{
    [JsonIgnore]
    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("Value of a failure result can't be accessed.");

    [JsonPropertyName("value")]
    public T? SerializableValue => IsSuccess ? value : default;

    [JsonPropertyName("isSuccess")]
    public new bool IsSuccess => base.IsSuccess;

    [JsonPropertyName("error")]
    public new Error Error => base.Error;

    [JsonPropertyName("message")]
    public new string Message => base.Message;


    public static Result<T> Success(T value, string message = "") =>
        new(value, true, Error.None, message);

    public new static Result<T> Failure(Error error) =>
        new(default, false, error, error.Message);

    public new static Result<T> Failure(string message) =>
        new(default, false, Error.Custom(message), message);
}