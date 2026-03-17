using FluentAssertions;
using FluentValidation.TestHelper;
using Gathering.Application.Sessions.Create;

namespace Gathering.Application.Tests.Sessions;

public class CreateSessionCommandValidatorTests
{
  private readonly CreateSessionCommandValidator _sut = new();
  private static readonly Guid ValidCommunityId = Guid.NewGuid();
  private const string ValidTitle = "Unit Testing Best Practices";
  private const string ValidSpeaker = "Jane Doe";
  private static readonly DateTimeOffset FutureDate = DateTimeOffset.UtcNow.AddDays(7);

  [Fact]
  public void Validate_WithValidCommand_ShouldHaveNoErrors()
  {
    var command = new CreateSessionCommand(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate);

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithEmptyCommunityId_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(Guid.Empty, ValidTitle, ValidSpeaker, FutureDate);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.CommunityId);
  }

  [Fact]
  public void Validate_WithEmptyTitle_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(ValidCommunityId, "", ValidSpeaker, FutureDate);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public void Validate_WithTitleExceeding200Chars_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(ValidCommunityId, new string('A', 201), ValidSpeaker, FutureDate);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title);
  }

  [Fact]
  public void Validate_WithDescriptionExceeding1000Chars_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(
        ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate, new string('A', 1001));

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Description);
  }

  [Fact]
  public void Validate_WithEmptySpeaker_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(ValidCommunityId, ValidTitle, "", FutureDate);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Speaker);
  }

  [Fact]
  public void Validate_WithSpeakerExceeding100Chars_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(ValidCommunityId, ValidTitle, new string('A', 101), FutureDate);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Speaker);
  }

  [Fact]
  public void Validate_WithPastScheduledAt_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(
        ValidCommunityId, ValidTitle, ValidSpeaker, DateTimeOffset.UtcNow.AddDays(-1));

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.ScheduledAt);
  }

  [Fact]
  public void Validate_WithImageStreamAndValidFields_ShouldHaveNoErrors()
  {
    var command = new CreateSessionCommand(
        ValidCommunityId,
        ValidTitle,
        ValidSpeaker,
        FutureDate,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "photo.jpg",
        ImageContentType: "image/jpeg");

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithImageStreamButMissingContentType_ShouldHaveValidationError()
  {
    var command = new CreateSessionCommand(
        ValidCommunityId,
        ValidTitle,
        ValidSpeaker,
        FutureDate,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "photo.jpg",
        ImageContentType: null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.ImageContentType);
  }
}
