using FluentAssertions;
using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Delete;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Communities;

public class DeleteCommunityCommandHandlerTests
{
  private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IImageStorageService _imageStorageService = Substitute.For<IImageStorageService>();
  private readonly DeleteCommunityCommandHandler _sut;

  public DeleteCommunityCommandHandlerTests()
  {
    _sut = new DeleteCommunityCommandHandler(_communityRepository, _unitOfWork, _imageStorageService);
  }

  private static Community CreateCommunity(string? image = null) =>
      Community.Create("Test Community", "A valid community description.", image).Value;

  [Fact]
  public async Task HandleAsync_WhenCommunityExists_ShouldDeleteAndReturnSuccess()
  {
    // Arrange
    var community = CreateCommunity();
    var command = new DeleteCommunityCommand(community.Id);
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    _communityRepository.Received(1).Remove(community);
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityNotFound_ShouldReturnFailure()
  {
    // Arrange
    var command = new DeleteCommunityCommand(Guid.NewGuid());
    _communityRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns((Community?)null);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NotFound);
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityHasBlobImage_ShouldDeleteImage()
  {
    // Arrange
    var blobUrl = "https://blob.core.windows.net/communities/img.jpg";
    var community = CreateCommunity(blobUrl);
    var command = new DeleteCommunityCommand(community.Id);
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);

    // Act
    await _sut.HandleAsync(command);

    // Assert
    await _imageStorageService.Received(1).DeleteImageAsync(blobUrl, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityHasNoBlobImage_ShouldNotCallDeleteImage()
  {
    // Arrange
    var community = CreateCommunity(); // no image
    var command = new DeleteCommunityCommand(community.Id);
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);

    // Act
    await _sut.HandleAsync(command);

    // Assert
    await _imageStorageService.DidNotReceive().DeleteImageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityHasExternalImage_ShouldNotCallDeleteImage()
  {
    // Arrange
    var community = CreateCommunity("https://images.unsplash.com/photo.jpg");
    var command = new DeleteCommunityCommand(community.Id);
    _communityRepository.GetByIdAsync(community.Id, Arg.Any<CancellationToken>())
        .Returns(community);

    // Act
    await _sut.HandleAsync(command);

    // Assert
    await _imageStorageService.DidNotReceive().DeleteImageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
  }
}
