using FluentAssertions;
using Gathering.SharedKernel;

namespace Gathering.SharedKernel.Tests;

public class ResultTests
{
  // ── Result (non-generic) ─────────────────────────────────────────────────

  [Fact]
  public void Success_ShouldReturnSuccessResult()
  {
    var result = Result.Success();

    result.IsSuccess.Should().BeTrue();
    result.IsFailure.Should().BeFalse();
    result.Error.Should().Be(Error.None);
  }

  [Fact]
  public void Failure_ShouldReturnFailureResult()
  {
    var error = Error.Failure("Test.Error", "Something went wrong");

    var result = Result.Failure(error);

    result.IsSuccess.Should().BeFalse();
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(error);
  }

  [Fact]
  public void Constructor_WhenSuccessWithNonNoneError_ShouldThrowArgumentException()
  {
    var error = Error.Failure("Test.Error", "Something went wrong");

    var act = () => new Result(true, error);

    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void Constructor_WhenFailureWithNoneError_ShouldThrowArgumentException()
  {
    var act = () => new Result(false, Error.None);

    act.Should().Throw<ArgumentException>();
  }

  // ── Result<TValue> ──────────────────────────────────────────────────────

  [Fact]
  public void GenericSuccess_ShouldReturnSuccessResultWithValue()
  {
    var result = Result.Success(42);

    result.IsSuccess.Should().BeTrue();
    result.Value.Should().Be(42);
    result.Error.Should().Be(Error.None);
  }

  [Fact]
  public void GenericFailure_ShouldReturnFailureResult()
  {
    var error = Error.NotFound("Test.NotFound", "Not found");

    var result = Result.Failure<int>(error);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(error);
  }

  [Fact]
  public void Value_WhenResultIsFailure_ShouldThrowInvalidOperationException()
  {
    var error = Error.Failure("Test.Error", "error");
    var result = Result.Failure<string>(error);

    var act = () => result.Value;

    act.Should().Throw<InvalidOperationException>();
  }

  [Fact]
  public void ImplicitConversion_FromNonNullValue_ShouldReturnSuccessResult()
  {
    Result<string> result = "hello";

    result.IsSuccess.Should().BeTrue();
    result.Value.Should().Be("hello");
  }

  [Fact]
  public void ImplicitConversion_FromNullValue_ShouldReturnFailureWithNullValueError()
  {
    Result<string> result = (string?)null;

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(Error.NullValue);
  }

  [Fact]
  public void ValidationFailure_ShouldReturnFailureResult()
  {
    var error = Error.Validation("Test.Validation", "Validation failed");

    var result = Result<string>.ValidationFailure(error);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(error);
    result.Error.Type.Should().Be(ErrorType.Validation);
  }
}
