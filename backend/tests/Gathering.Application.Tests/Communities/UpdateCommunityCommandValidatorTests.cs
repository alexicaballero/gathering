using FluentAssertions;
using FluentValidation.TestHelper;
using Gathering.Application.Communities.Update;

namespace Gathering.Application.Tests.Communities;

public class UpdateCommunityCommandValidatorTests
{
  private readonly UpdateCommunityCommandValidator _sut = new();
  private static readonly Guid ValidId = Guid.NewGuid();
  private const string ValidName = "Valid Community";
  private const string ValidDescription = "A valid description for the community.";

  [Fact]
  public void Validate_WithValidCommand_ShouldHaveNoErrors()
  {
    var command = new UpdateCommunityCommand(ValidId, ValidName, ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithEmptyCommunityId_ShouldHaveValidationError()
  {
    var command = new UpdateCommunityCommand(Guid.Empty, ValidName, ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.CommunityId);
  }

  [Fact]
  public void Validate_WithEmptyName_ShouldHaveValidationError()
  {
    var command = new UpdateCommunityCommand(ValidId, "", ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Name);
  }

  [Fact]
  public void Validate_WithNameExceeding200Chars_ShouldHaveValidationError()
  {
    var command = new UpdateCommunityCommand(ValidId, new string('A', 201), ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Name);
  }

  [Fact]
  public void Validate_WithEmptyDescription_ShouldHaveValidationError()
  {
    var command = new UpdateCommunityCommand(ValidId, ValidName, "");

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Description);
  }

  [Fact]
  public void Validate_WithDescriptionExceeding1000Chars_ShouldHaveValidationError()
  {
    var command = new UpdateCommunityCommand(ValidId, ValidName, new string('A', 1001));

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Description);
  }

  [Fact]
  public void Validate_WithImageStreamAndValidFields_ShouldHaveNoErrors()
  {
    var command = new UpdateCommunityCommand(
        ValidId,
        ValidName,
        ValidDescription,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "photo.jpg",
        ImageContentType: "image/jpeg");

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithImageStreamButMissingFileName_ShouldHaveValidationError()
  {
    var command = new UpdateCommunityCommand(
        ValidId,
        ValidName,
        ValidDescription,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: null,
        ImageContentType: "image/jpeg");

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.ImageFileName);
  }
}
