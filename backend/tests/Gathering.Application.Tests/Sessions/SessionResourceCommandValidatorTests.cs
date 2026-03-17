using FluentAssertions;
using FluentValidation.TestHelper;
using Gathering.Application.Sessions.Resources.Create;
using Gathering.Application.Sessions.Resources.Update;
using Gathering.Domain.Sessions;

namespace Gathering.Application.Tests.Sessions;

public class CreateSessionResourceCommandValidatorTests
{
  private readonly CreateSessionResourceCommandValidator _sut = new();
  private static readonly Guid ValidSessionId = Guid.NewGuid();

  [Fact]
  public void Validate_WithValidVideoCommand_ShouldHaveNoErrors()
  {
    var command = new CreateSessionResourceCommand(
        ValidSessionId, SessionResourceType.Video, "https://youtube.com", null, "My Video");

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithEmptySessionId_ShouldHaveValidationError()
  {
    var command = new CreateSessionResourceCommand(
        Guid.Empty, SessionResourceType.Video, "https://youtube.com", null, null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.SessionId);
  }

  [Fact]
  public void Validate_WithInvalidType_ShouldHaveValidationError()
  {
    var command = new CreateSessionResourceCommand(
        ValidSessionId, (SessionResourceType)99, null, null, null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Type);
  }

  [Fact]
  public void Validate_WithTitleExceeding200Chars_ShouldHaveValidationError()
  {
    var command = new CreateSessionResourceCommand(
        ValidSessionId, SessionResourceType.Video, "https://youtube.com", null, new string('A', 201));

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public void Validate_WithUrlExceeding2000Chars_ShouldHaveValidationError()
  {
    var command = new CreateSessionResourceCommand(
        ValidSessionId,
        SessionResourceType.Video,
        "https://example.com/" + new string('a', 1985),
        null,
        null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Url);
  }

  [Fact]
  public void Validate_WithNotesExceeding4000Chars_ShouldHaveValidationError()
  {
    var command = new CreateSessionResourceCommand(
        ValidSessionId, SessionResourceType.Notes, null, new string('A', 4001), null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Notes);
  }

  [Fact]
  public void Validate_WithValidNotesCommand_ShouldHaveNoErrors()
  {
    var command = new CreateSessionResourceCommand(
        ValidSessionId, SessionResourceType.Notes, null, "Some notes content", "Notes");

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }
}

public class UpdateSessionResourceCommandValidatorTests
{
  private readonly UpdateSessionResourceCommandValidator _sut = new();
  private static readonly Guid ValidSessionId = Guid.NewGuid();
  private static readonly Guid ValidResourceId = Guid.NewGuid();

  [Fact]
  public void Validate_WithValidCommand_ShouldHaveNoErrors()
  {
    var command = new UpdateSessionResourceCommand(
        ValidSessionId, ValidResourceId, "https://youtube.com/new", null, "Updated Title");

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithEmptySessionId_ShouldHaveValidationError()
  {
    var command = new UpdateSessionResourceCommand(Guid.Empty, ValidResourceId, null, null, null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.SessionId);
  }

  [Fact]
  public void Validate_WithEmptyResourceId_ShouldHaveValidationError()
  {
    var command = new UpdateSessionResourceCommand(ValidSessionId, Guid.Empty, null, null, null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.ResourceId);
  }

  [Fact]
  public void Validate_WithTitleExceeding200Chars_ShouldHaveValidationError()
  {
    var command = new UpdateSessionResourceCommand(
        ValidSessionId, ValidResourceId, null, null, new string('A', 201));

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public void Validate_WithUrlExceeding2000Chars_ShouldHaveValidationError()
  {
    var command = new UpdateSessionResourceCommand(
        ValidSessionId,
        ValidResourceId,
        "https://example.com/" + new string('a', 1985),
        null,
        null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Url);
  }

  [Fact]
  public void Validate_WithNotesExceeding4000Chars_ShouldHaveValidationError()
  {
    var command = new UpdateSessionResourceCommand(
        ValidSessionId, ValidResourceId, null, new string('A', 4001), null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Notes);
  }
}
