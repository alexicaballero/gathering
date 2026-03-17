using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Update;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Communities;

public class UpdateCommunityCommandHandlerTests
{
  private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IValidator<UpdateCommunityCommand> _validator = Substitute.For<IValidator<UpdateCommunityCommand>>();
  private readonly IImageStorageService _imageStorageService = Substitute.For<IImageStorageService>();
  private readonly UpdateCommunityCommandHandler _sut;

  public UpdateCommunityCommandHandlerTests()
  {
    _sut = new UpdateCommunityCommandHandler(
        _communityRepository,
        _unitOfWork,
        _validator,
        _imageStorageService);
  }

  private static Community CreateCommunity(string? image = null)
  {
    var result = Community.Create("Original Name", "Original description text here.", image);
    return result.Value;
  }

  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccess()
  {
    // Arrange
    var community = CreateCommunity();
    var command = new UpdateCommunityCommand(community.Id, "Updated Name", "Updated description.");
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityNotFound_ShouldReturnFailure()
  {
    // Arrange
    var command = new UpdateCommunityCommand(Guid.NewGuid(), "Name", "Description.");
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns((Community?)null);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NotFound);
  }

  [Fact]
  public async Task HandleAsync_WhenValidationFails_ShouldReturnFailure()
  {
    // Arrange
    var command = new UpdateCommunityCommand(Guid.NewGuid(), "", "Description.");
    var failures = new List<ValidationFailure> { new("Name", "Name is required.") };
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult(failures));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    _communityRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
  }

  [Fact]
  public async Task HandleAsync_WithNewImage_ShouldUploadAndDeleteOldBlobImage()
  {
    // Arrange
    var oldImageUrl = "https://blob.core.windows.net/communities/old.jpg";
    var newImageUrl = "https://blob.core.windows.net/communities/new.jpg";
    var community = CreateCommunity(oldImageUrl);
    var command = new UpdateCommunityCommand(
        community.Id,
        "Updated Name",
        "Updated description.",
        new MemoryStream([1, 2, 3]),
        "new.jpg",
        "image/jpeg");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);
    _imageStorageService.UploadImageAsync(
            Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Result.Success(newImageUrl));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    await _imageStorageService.Received(1).DeleteImageAsync(oldImageUrl, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WithNewImageAndOldExternalUrl_ShouldNotDeleteOldImage()
  {
    // Arrange — old image is from Unsplash, not blob
    var externalImageUrl = "https://images.unsplash.com/photo.jpg";
    var newImageUrl = "https://blob.core.windows.net/communities/new.jpg";
    var community = CreateCommunity(externalImageUrl);
    var command = new UpdateCommunityCommand(
        community.Id,
        "Updated Name",
        "Updated description.",
        new MemoryStream([1, 2, 3]),
        "new.jpg",
        "image/jpeg");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);
    _imageStorageService.UploadImageAsync(
            Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Result.Success(newImageUrl));

    // Act
    await _sut.HandleAsync(command);

    // Assert — DeleteImageAsync must NOT be called for non-blob URLs
    await _imageStorageService.DidNotReceive().DeleteImageAsync(externalImageUrl, Arg.Any<CancellationToken>());
  }
}
