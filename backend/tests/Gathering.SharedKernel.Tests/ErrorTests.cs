using FluentAssertions;
using Gathering.SharedKernel;

namespace Gathering.SharedKernel.Tests;

public class ErrorTests
{
  [Fact]
  public void Failure_ShouldCreateErrorWithFailureType()
  {
    var error = Error.Failure("Test.Code", "Test description");

    error.Code.Should().Be("Test.Code");
    error.Description.Should().Be("Test description");
    error.Type.Should().Be(ErrorType.Failure);
  }

  [Fact]
  public void Validation_ShouldCreateErrorWithValidationType()
  {
    var error = Error.Validation("Test.Validation", "Validation error");

    error.Type.Should().Be(ErrorType.Validation);
  }

  [Fact]
  public void Problem_ShouldCreateErrorWithProblemType()
  {
    var error = Error.Problem("Test.Problem", "Problem occurred");

    error.Type.Should().Be(ErrorType.Problem);
  }

  [Fact]
  public void NotFound_ShouldCreateErrorWithNotFoundType()
  {
    var error = Error.NotFound("Test.NotFound", "Not found");

    error.Type.Should().Be(ErrorType.NotFound);
  }

  [Fact]
  public void Conflict_ShouldCreateErrorWithConflictType()
  {
    var error = Error.Conflict("Test.Conflict", "Conflict");

    error.Type.Should().Be(ErrorType.Conflict);
  }

  [Fact]
  public void None_ShouldHaveEmptyCodeAndDescription()
  {
    Error.None.Code.Should().BeEmpty();
    Error.None.Description.Should().BeEmpty();
    Error.None.Type.Should().Be(ErrorType.Failure);
  }

  [Fact]
  public void NullValue_ShouldHaveExpectedCodeAndFailureType()
  {
    Error.NullValue.Code.Should().Be("General.Null");
    Error.NullValue.Type.Should().Be(ErrorType.Failure);
  }

  [Fact]
  public void TwoErrorsWithSameValues_ShouldBeEqual()
  {
    var error1 = Error.Validation("Code", "Desc");
    var error2 = Error.Validation("Code", "Desc");

    error1.Should().Be(error2);
  }

  [Fact]
  public void TwoErrorsWithDifferentCodes_ShouldNotBeEqual()
  {
    var error1 = Error.Validation("Code1", "Desc");
    var error2 = Error.Validation("Code2", "Desc");

    error1.Should().NotBe(error2);
  }
}
