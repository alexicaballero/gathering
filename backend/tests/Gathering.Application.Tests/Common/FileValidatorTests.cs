using FluentAssertions;
using Gathering.Application.Common.Validators;
using Gathering.SharedKernel;

namespace Gathering.Application.Tests.Common;

public class FileValidatorTests
{
  private static Stream MakeStream(int size)
  {
    var bytes = new byte[size];
    new Random().NextBytes(bytes);
    return new MemoryStream(bytes);
  }

  [Theory]
  [InlineData("image/jpeg", "photo.jpg")]
  [InlineData("image/png", "photo.png")]
  [InlineData("image/gif", "photo.gif")]
  [InlineData("image/webp", "photo.webp")]
  public void ValidateImageFile_WithAllowedContentTypeAndExtension_ShouldReturnSuccess(
      string contentType, string fileName)
  {
    var stream = MakeStream(1024);

    var result = FileValidator.ValidateImageFile(stream, fileName, contentType);

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void ValidateImageFile_WithInvalidContentType_ShouldReturnFailure()
  {
    var stream = MakeStream(1024);

    var result = FileValidator.ValidateImageFile(stream, "file.pdf", "application/pdf");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(ImageValidationError.InvalidContentType);
  }

  [Fact]
  public void ValidateImageFile_WithInvalidExtension_ShouldReturnFailure()
  {
    var stream = MakeStream(1024);

    var result = FileValidator.ValidateImageFile(stream, "file.bmp", "image/jpeg");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(ImageValidationError.InvalidExtension);
  }

  [Fact]
  public void ValidateImageFile_WithFileExceeding10MB_ShouldReturnFailure()
  {
    const int tenMBPlusOne = 10 * 1024 * 1024 + 1;
    var largeStream = MakeStream(tenMBPlusOne);

    var result = FileValidator.ValidateImageFile(largeStream, "photo.jpg", "image/jpeg");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(ImageValidationError.FileTooLarge);
  }

  [Fact]
  public void ValidateImageFile_WithFileExactly10MB_ShouldReturnSuccess()
  {
    const int tenMB = 10 * 1024 * 1024;
    var stream = MakeStream(tenMB);

    var result = FileValidator.ValidateImageFile(stream, "photo.jpg", "image/jpeg");

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void ValidateImageFile_ContentTypeIsCaseInsensitive()
  {
    var stream = MakeStream(1024);

    var result = FileValidator.ValidateImageFile(stream, "photo.jpg", "IMAGE/JPEG");

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void ValidateImageFile_ExtensionIsCaseInsensitive()
  {
    var stream = MakeStream(1024);

    var result = FileValidator.ValidateImageFile(stream, "photo.JPG", "image/jpeg");

    result.IsSuccess.Should().BeTrue();
  }
}
