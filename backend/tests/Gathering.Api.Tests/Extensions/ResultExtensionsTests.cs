using FluentAssertions;
using Gathering.Api.Extensions;
using Gathering.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Gathering.Api.Tests.Extensions;

public class ResultExtensionsTests
{
  // ── Result (non-generic) ─────────────────────────────────────────────────

  [Fact]
  public void Match_WhenResultIsSuccess_ShouldCallOnSuccess()
  {
    var result = Result.Success();
    var called = false;

    result.Match(
        onSuccess: () => { called = true; return "ok"; },
        onFailure: _ => "fail");

    called.Should().BeTrue();
  }

  [Fact]
  public void Match_WhenResultIsFailure_ShouldCallOnFailure()
  {
    var error = Error.Failure("Test.Error", "error");
    var result = Result.Failure(error);
    Result? capturedResult = null;

    result.Match(
        onSuccess: () => "ok",
        onFailure: r => { capturedResult = r; return "fail"; });

    capturedResult.Should().NotBeNull();
    capturedResult!.Error.Should().Be(error);
  }

  [Fact]
  public void Match_WhenResultIsSuccess_ShouldReturnOnSuccessValue()
  {
    var result = Result.Success();

    var output = result.Match(
        onSuccess: () => 42,
        onFailure: _ => -1);

    output.Should().Be(42);
  }

  [Fact]
  public void Match_WhenResultIsFailure_ShouldReturnOnFailureValue()
  {
    var result = Result.Failure(Error.Failure("E", "msg"));

    var output = result.Match(
        onSuccess: () => 42,
        onFailure: _ => -1);

    output.Should().Be(-1);
  }

  // ── Result<T> (generic) ──────────────────────────────────────────────────

  [Fact]
  public void GenericMatch_WhenResultIsSuccess_ShouldCallOnSuccessWithValue()
  {
    var result = Result.Success("hello");
    string? received = null;

    result.Match(
        onSuccess: v => { received = v; return "ok"; },
        onFailure: _ => "fail");

    received.Should().Be("hello");
  }

  [Fact]
  public void GenericMatch_WhenResultIsFailure_ShouldCallOnFailure()
  {
    var error = Error.NotFound("Test.NotFound", "not found");
    var result = Result.Failure<string>(error);
    var wasCalled = false;

    result.Match(
        onSuccess: _ => "ok",
        onFailure: _ => { wasCalled = true; return "fail"; });

    wasCalled.Should().BeTrue();
  }

  [Fact]
  public void GenericMatch_WhenResultIsSuccess_ShouldReturnOnSuccessValue()
  {
    var result = Result.Success(99);

    var output = result.Match(
        onSuccess: v => v * 2,
        onFailure: _ => 0);

    output.Should().Be(198);
  }
}
