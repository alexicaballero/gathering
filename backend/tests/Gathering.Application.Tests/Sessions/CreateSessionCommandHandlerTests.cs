using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Create;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Sessions;

public class CreateSessionCommandHandlerTests
{
  private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
  private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IValidator<CreateSessionCommand> _validator = Substitute.For<IValidator<CreateSessionCommand>>();
  private readonly IImageStorageService _imageStorageService = Substitute.For<IImageStorageService>();
  private readonly CreateSessionCommandHandler _sut;

  public CreateSessionCommandHandlerTests()
  {
    _sut = new CreateSessionCommandHandler(
        _sessionRepository,
        _communityRepository,
        _unitOfWork,
        _validator,
        _imageStorageService);
  }

  private static CreateSessionCommand ValidCommand(Guid communityId) =>
      new(communityId, "My Session", "A Speaker", DateTimeOffset.UtcNow.AddDays(7));

  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccessWithId()
  {
    // Arrange
    var communityId = Guid.NewGuid();
    var command = ValidCommand(communityId);
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.ExistsAsync(communityId, Arg.Any<CancellationToken>())
        .Returns(true);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
    _sessionRepository.Received(1).Add(Arg.Any<Session>());
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenValidationFails_ShouldReturnFailure()
  {
    // Arrange
    var command = ValidCommand(Guid.NewGuid());
    var failures = new List<ValidationFailure> { new("Title", "Title is required.") };
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult(failures));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Validation);
    _sessionRepository.DidNotReceive().Add(Arg.Any<Session>());
  }

  [Fact]
  public async Task HandleAsync_WhenCommunityNotFound_ShouldReturnFailure()
  {
    // Arrange
    var communityId = Guid.NewGuid();
    var command = ValidCommand(communityId);
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.ExistsAsync(communityId, Arg.Any<CancellationToken>())
        .Returns(false);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NotFound);
  }

  [Fact]
  public async Task HandleAsync_WithImage_ShouldUploadImageAndReturnSuccess()
  {
    // Arrange
    var communityId = Guid.NewGuid();
    var imageUrl = "https://blob.core.windows.net/sessions/img.jpg";
    var command = new CreateSessionCommand(
        communityId,
        "My Session",
        "A Speaker",
        DateTimeOffset.UtcNow.AddDays(7),
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "img.jpg",
        ImageContentType: "image/jpeg");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.ExistsAsync(communityId, Arg.Any<CancellationToken>())
        .Returns(true);
    _imageStorageService.UploadImageAsync(
            Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Result.Success(imageUrl));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    await _imageStorageService.Received(1).UploadImageAsync(
        Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), "sessions", Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenImageUploadFails_ShouldReturnFailure()
  {
    // Arrange
    var communityId = Guid.NewGuid();
    var command = new CreateSessionCommand(
        communityId,
        "My Session",
        "A Speaker",
        DateTimeOffset.UtcNow.AddDays(7),
        ImageStream: new MemoryStream([1, 2, 3]),
        ImageFileName: "img.jpg",
        ImageContentType: "image/jpeg");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _communityRepository.ExistsAsync(communityId, Arg.Any<CancellationToken>())
        .Returns(true);
    _imageStorageService.UploadImageAsync(
            Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
        .Returns(Result.Failure<string>(SessionError.ImageUploadFailed));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ImageUploadFailed);
  }
}
