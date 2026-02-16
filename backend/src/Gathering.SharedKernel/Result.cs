using System.Diagnostics.CodeAnalysis;

namespace Gathering.SharedKernel;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);

    /// <summary>
    /// Executes one of two functions based on the result state
    /// </summary>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);

    /// <summary>
    /// Executes one of two functions based on the result state
    /// </summary>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Error);

    /// <summary>
    /// Transforms the value of a successful result
    /// </summary>
    public Result<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> transform) =>
        IsSuccess ? Success(transform(Value)) : Failure<TNewValue>(Error);

    /// <summary>
    /// Transforms the error of a failed result
    /// </summary>
    public Result<TValue> MapError(Func<Error, Error> transform) =>
        IsFailure ? Failure<TValue>(transform(Error)) : this;

    /// <summary>
    /// Chains another result-returning operation if this result is successful
    /// </summary>
    public Result<TNewValue> Then<TNewValue>(Func<TValue, Result<TNewValue>> next) =>
        IsSuccess ? next(Value) : Failure<TNewValue>(Error);

    /// <summary>
    /// Chains another result-returning operation if this result is successful
    /// </summary>
    public Result Then(Func<TValue, Result> next) =>
        IsSuccess ? next(Value) : Failure(Error);
}
