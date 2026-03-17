using FluentAssertions;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Delete;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Sessions;

public class DeleteSessionCommandHandlerTests
{
  private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IImageStorageService _imageStorageService = Substitute.For<IImageStorageService>();
  private readonly DeleteSessionCommandHandler _sut;

  public DeleteSessionCommandHandlerTests()
  {
    _sut = new DeleteSessionCommandHandler(_sessionRepository, _unitOfWork, _imageStorageService);
  }

  private static Session CreateSession(string? image = null) =>
      Session.Create(Guid.NewGuid(), "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(7), image: image).Value;

  [Fact]
  public async Task HandleAsync_WhenSessionExists_ShouldDeleteAndReturnSuccess()
  {
    // Arrange
    var session = CreateSession();
    var command = new DeleteSessionCommand(session.Id);
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    _sessionRepository.Received(1).Remove(session);
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenSessionNotFound_ShouldReturnFailure()
  {
    // Arrange
    var command = new DeleteSessionCommand(Guid.NewGuid());
    _sessionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns((Session?)null);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.NotFound);
  }

  [Fact]
  public async Task HandleAsync_WhenSessionHasBlobImage_ShouldDeleteImage()
  {
    // Arrange
    var blobUrl = "https://blob.core.windows.net/sessions/img.jpg";
    var session = CreateSession(blobUrl);
    var command = new DeleteSessionCommand(session.Id);
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    await _sut.HandleAsync(command);

    // Assert
    await _imageStorageService.Received(1).DeleteImageAsync(blobUrl, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenSessionHasExternalImage_ShouldNotDeleteImage()
  {
    // Arrange
    var session = CreateSession("https://images.unsplash.com/photo.jpg");
    var command = new DeleteSessionCommand(session.Id);
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    await _sut.HandleAsync(command);

    // Assert
    await _imageStorageService.DidNotReceive().DeleteImageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
  }
}
