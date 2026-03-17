using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Update;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Sessions;

public class UpdateSessionCommandHandlerTests
{
  private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IValidator<UpdateSessionCommand> _validator = Substitute.For<IValidator<UpdateSessionCommand>>();
  private readonly IImageStorageService _imageStorageService = Substitute.For<IImageStorageService>();
  private readonly UpdateSessionCommandHandler _sut;

  public UpdateSessionCommandHandlerTests()
  {
    _sut = new UpdateSessionCommandHandler(_sessionRepository, _unitOfWork, _validator, _imageStorageService);
  }

  private static Session CreateSession(string? image = null) =>
      Session.Create(Guid.NewGuid(), "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(7), image: image).Value;

  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccess()
  {
    // Arrange
    var session = CreateSession();
    var command = new UpdateSessionCommand(
        session.Id, "Updated Title", "Updated Speaker", DateTimeOffset.UtcNow.AddDays(14), SessionStatus.Scheduled);

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenSessionNotFound_ShouldReturnFailure()
  {
    // Arrange
    var command = new UpdateSessionCommand(
        Guid.NewGuid(), "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(7), SessionStatus.Scheduled);

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns((Session?)null);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.NotFound);
  }

  [Fact]
  public async Task HandleAsync_WhenValidationFails_ShouldReturnFailure()
  {
    // Arrange
    var command = new UpdateSessionCommand(
        Guid.NewGuid(), "", "Speaker", DateTimeOffset.UtcNow.AddDays(7), SessionStatus.Scheduled);
    var failures = new List<ValidationFailure> { new("Title", "Title is required.") };
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult(failures));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Validation);
  }

  [Fact]
  public async Task HandleAsync_WithStatusUpdate_ShouldUpdateStatus()
  {
    // Arrange
    var session = CreateSession();
    var command = new UpdateSessionCommand(
        session.Id, "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(7), SessionStatus.Completed);

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    session.Status.Should().Be(SessionStatus.Completed);
  }

  [Fact]
  public async Task HandleAsync_WithNewBlobImageReplacement_ShouldDeleteOldImage()
  {
    // Arrange
    var oldBlobUrl = "https://blob.core.windows.net/sessions/old.jpg";
    var newBlobUrl = "https://blob.core.windows.net/sessions/new.jpg";
    var session = CreateSession(oldBlobUrl);
    var command = new UpdateSessionCommand(
        session.Id,
        "Title",
        "Speaker",
        DateTimeOffset.UtcNow.AddDays(7),
        SessionStatus.Scheduled,
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "new.jpg",
        ImageContentType: "image/jpeg");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);
    _imageStorageService.UploadImageAsync(
            Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Result.Success(newBlobUrl));

    // Act
    await _sut.HandleAsync(command);

    // Assert
    await _imageStorageService.Received(1).DeleteImageAsync(oldBlobUrl, Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenInvalidStatusTransition_ShouldReturnFailure()
  {
    // Arrange — session is Canceled, trying to go back to Scheduled
    var session = CreateSession();
    session.UpdateStatus(SessionStatus.Canceled);
    var command = new UpdateSessionCommand(
        session.Id, "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(7), SessionStatus.Scheduled);

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.InvalidStatusTransition);
  }
}
