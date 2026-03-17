using FluentAssertions;
using FluentValidation.TestHelper;
using Gathering.Application.Sessions.Update;
using Gathering.Domain.Sessions;

namespace Gathering.Application.Tests.Sessions;

public class UpdateSessionCommandValidatorTests
{
  private readonly UpdateSessionCommandValidator _sut = new();
  private static readonly Guid ValidSessionId = Guid.NewGuid();
  private const string ValidTitle = "Clean Architecture Deep Dive";
  private const string ValidSpeaker = "Uncle Bob";
  private static readonly DateTimeOffset FutureDate = DateTimeOffset.UtcNow.AddDays(7);

  [Fact]
  public void Validate_WithValidCommand_ShouldHaveNoErrors()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId, ValidTitle, ValidSpeaker, FutureDate, SessionStatus.Scheduled);

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithEmptySessionId_ShouldHaveValidationError()
  {
    var command = new UpdateSessionCommand(
        Guid.Empty, ValidTitle, ValidSpeaker, FutureDate, SessionStatus.Scheduled);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.SessionId);
  }

  [Fact]
  public void Validate_WithEmptyTitle_ShouldHaveValidationError()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId, "", ValidSpeaker, FutureDate, SessionStatus.Scheduled);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public void Validate_WithTitleExceeding200Chars_ShouldHaveValidationError()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId, new string('A', 201), ValidSpeaker, FutureDate, SessionStatus.Scheduled);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public void Validate_WithDescriptionExceeding1000Chars_ShouldHaveValidationError()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId,
        ValidTitle,
        ValidSpeaker,
        FutureDate,
        SessionStatus.Scheduled,
        Description: new string('A', 1001));

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Description);
  }

  [Fact]
  public void Validate_WithEmptySpeaker_ShouldHaveValidationError()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId, ValidTitle, "", FutureDate, SessionStatus.Scheduled);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Speaker);
  }

  [Fact]
  public void Validate_WithInvalidStatus_ShouldHaveValidationError()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId, ValidTitle, ValidSpeaker, FutureDate, (SessionStatus)99);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Status);
  }

  [Fact]
  public void Validate_WithCompletedStatus_ShouldHaveNoErrors()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId, ValidTitle, ValidSpeaker, FutureDate, SessionStatus.Completed);

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithImageStreamAndValidFields_ShouldHaveNoErrors()
  {
    var command = new UpdateSessionCommand(
        ValidSessionId,
        ValidTitle,
        ValidSpeaker,
        FutureDate,
        SessionStatus.Scheduled,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "photo.jpg",
        ImageContentType: "image/jpeg");

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }
}
