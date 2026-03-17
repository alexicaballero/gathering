using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Create;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Communities;

public class CreateCommunityCommandHandlerTests
{
  private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IValidator<CreateCommunityCommand> _validator = Substitute.For<IValidator<CreateCommunityCommand>>();
  private readonly IImageStorageService _imageStorageService = Substitute.For<IImageStorageService>();
  private readonly CreateCommunityCommandHandler _sut;

  public CreateCommunityCommandHandlerTests()
  {
    _sut = new CreateCommunityCommandHandler(
        _communityRepository,
        _unitOfWork,
        _validator,
        _imageStorageService);
  }

  [Fact]
  public async Task HandleAsync_WithValidCommandAndNoImage_ShouldReturnSuccessWithId()
  {
    // Arrange
    var command = new CreateCommunityCommand("Test Community", "A valid community description.");
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
    _communityRepository.Received(1).Add(Arg.Any<Community>());
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WithValidCommandAndImage_ShouldUploadImageAndReturnSuccess()
  {
    // Arrange
    var imageUrl = "https://blob.core.windows.net/communities/img.jpg";
    var command = new CreateCommunityCommand(
        "Test Community",
        "A valid community description.",
        new MemoryStream([1, 2, 3]),
        "photo.jpg",
        "image/jpeg");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _imageStorageService.UploadImageAsync(
            Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Result.Success(imageUrl));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    await _imageStorageService.Received(1).UploadImageAsync(
        Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), "communities", Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenValidationFails_ShouldReturnFailure()
  {
    // Arrange
    var command = new CreateCommunityCommand("", "desc");
    var validationFailures = new List<ValidationFailure>
        {
            new("Name", "Community name is required.")
        };
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult(validationFailures));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Validation);
    _communityRepository.DidNotReceive().Add(Arg.Any<Community>());
  }

  [Fact]
  public async Task HandleAsync_WhenImageUploadFails_ShouldReturnFailure()
  {
    // Arrange
    var command = new CreateCommunityCommand(
        "Test Community",
        "A valid community description.",
        new MemoryStream([1, 2, 3]),
        "photo.jpg",
        "image/jpeg");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _imageStorageService.UploadImageAsync(
            Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Result.Failure<string>(CommunityError.ImageUploadFailed));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.ImageUploadFailed);
  }
}
