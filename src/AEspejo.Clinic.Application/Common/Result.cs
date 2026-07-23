namespace AEspejo.Clinic.Application.Common;

/// <summary>Result status, mappable to HTTP status codes in the API layer.</summary>
public enum ResultStatus
{
    Ok,
    NotFound,
    Invalid,
    Conflict,
    Unauthorized
}

/// <summary>Result of an operation that returns a value.</summary>
public class Result<T>
{
    public ResultStatus Status { get; init; }
    public T? Value { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public bool IsSuccess => Status == ResultStatus.Ok;

    public static Result<T> Ok(T value) => new() { Status = ResultStatus.Ok, Value = value };
    public static Result<T> NotFound(string message = "Recurso no encontrado.") =>
        new() { Status = ResultStatus.NotFound, Errors = [message] };
    public static Result<T> Invalid(params string[] errors) =>
        new() { Status = ResultStatus.Invalid, Errors = errors };
    public static Result<T> Conflict(string message) =>
        new() { Status = ResultStatus.Conflict, Errors = [message] };
    public static Result<T> Unauthorized(string message = "No autorizado.") =>
        new() { Status = ResultStatus.Unauthorized, Errors = [message] };
}

/// <summary>Result of an operation without a return value.</summary>
public class Result
{
    public ResultStatus Status { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public bool IsSuccess => Status == ResultStatus.Ok;

    public static Result Ok() => new() { Status = ResultStatus.Ok };
    public static Result NotFound(string message = "Recurso no encontrado.") =>
        new() { Status = ResultStatus.NotFound, Errors = [message] };
    public static Result Invalid(params string[] errors) =>
        new() { Status = ResultStatus.Invalid, Errors = errors };
    public static Result Conflict(string message) =>
        new() { Status = ResultStatus.Conflict, Errors = [message] };
}
