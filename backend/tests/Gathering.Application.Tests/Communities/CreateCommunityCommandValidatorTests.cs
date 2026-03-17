using FluentAssertions;
using FluentValidation.TestHelper;
using Gathering.Application.Communities.Create;

namespace Gathering.Application.Tests.Communities;

public class CreateCommunityCommandValidatorTests
{
  private readonly CreateCommunityCommandValidator _sut = new();
  private static readonly string ValidName = "Valid Community";
  private static readonly string ValidDescription = "A valid description that meets requirements.";

  // ── Name ──────────────────────────────────────────────────────────────────

  [Fact]
  public void Validate_WithValidCommand_ShouldHaveNoErrors()
  {
    var command = new CreateCommunityCommand(ValidName, ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void Validate_WithEmptyName_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand("", ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Name);
  }

  [Fact]
  public void Validate_WithNameExceeding200Chars_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand(new string('A', 201), ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Name);
  }

  [Fact]
  public void Validate_WithNameContainingInvalidCharacters_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand("Invalid@Name!", ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Name);
  }

  [Fact]
  public void Validate_WithNameContainingValidCharacters_ShouldHaveNoError()
  {
    var command = new CreateCommunityCommand("Valid-Name_123.ok", ValidDescription);

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveValidationErrorFor(x => x.Name);
  }

  // ── Description ───────────────────────────────────────────────────────────

  [Fact]
  public void Validate_WithEmptyDescription_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand(ValidName, "");

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Description);
  }

  [Fact]
  public void Validate_WithDescriptionShorterThan10Chars_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand(ValidName, "Short");

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Description);
  }

  [Fact]
  public void Validate_WithDescriptionExceeding1000Chars_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand(ValidName, new string('A', 1001));

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Description);
  }

  // ── Image ─────────────────────────────────────────────────────────────────

  [Fact]
  public void Validate_WithImageStreamButNoFileName_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand(
        ValidName,
        ValidDescription,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: null,
        ImageContentType: "image/jpeg");

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.ImageFileName);
  }

  [Fact]
  public void Validate_WithImageStreamButNoContentType_ShouldHaveValidationError()
  {
    var command = new CreateCommunityCommand(
        ValidName,
        ValidDescription,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "photo.jpg",
        ImageContentType: null);

    var result = _sut.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.ImageContentType);
  }

  [Fact]
  public void Validate_WithValidImageFields_ShouldHaveNoErrors()
  {
    var command = new CreateCommunityCommand(
        ValidName,
        ValidDescription,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "photo.jpg",
        ImageContentType: "image/jpeg");

    var result = _sut.TestValidate(command);

    result.ShouldNotHaveAnyValidationErrors();
  }
}
