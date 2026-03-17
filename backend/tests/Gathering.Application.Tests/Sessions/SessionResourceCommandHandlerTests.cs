using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Resources.Create;
using Gathering.Application.Sessions.Resources.Delete;
using Gathering.Application.Sessions.Resources.Update;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Sessions;

public class CreateSessionResourceCommandHandlerTests
{
  private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IValidator<CreateSessionResourceCommand> _validator =
      Substitute.For<IValidator<CreateSessionResourceCommand>>();
  private readonly CreateSessionResourceCommandHandler _sut;

  public CreateSessionResourceCommandHandlerTests()
  {
    _sut = new CreateSessionResourceCommandHandler(_sessionRepository, _unitOfWork, _validator);
  }

  private static Session CreateSession() =>
      Session.Create(Guid.NewGuid(), "Test Session", "Speaker", DateTimeOffset.UtcNow.AddDays(7)).Value;

  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccessWithResourceId()
  {
    // Arrange
    var session = CreateSession();
    var command = new CreateSessionResourceCommand(
        session.Id, SessionResourceType.Video, "https://youtube.com", null, "Video");

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
    _sessionRepository.Received(1).AddResource(Arg.Any<SessionResource>());
    await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task HandleAsync_WhenValidationFails_ShouldReturnFailure()
  {
    // Arrange
    var command = new CreateSessionResourceCommand(Guid.Empty, SessionResourceType.Video, null, null, null);
    var failures = new List<ValidationFailure> { new("SessionId", "Session ID is required.") };
    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult(failures));

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Type.Should().Be(ErrorType.Validation);
  }

  [Fact]
  public async Task HandleAsync_WhenSessionNotFound_ShouldReturnFailure()
  {
    // Arrange
    var command = new CreateSessionResourceCommand(
        Guid.NewGuid(), SessionResourceType.Video, "https://youtube.com", null, null);

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
  public async Task HandleAsync_WhenDomainRuleFails_ShouldReturnFailure()
  {
    // Arrange — Video without a URL causes domain failure
    var session = CreateSession();
    var command = new CreateSessionResourceCommand(session.Id, SessionResourceType.Video, null, null, null);

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceUrlEmpty);
  }
}

public class UpdateSessionResourceCommandHandlerTests
{
  private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly IValidator<UpdateSessionResourceCommand> _validator =
      Substitute.For<IValidator<UpdateSessionResourceCommand>>();
  private readonly UpdateSessionResourceCommandHandler _sut;

  public UpdateSessionResourceCommandHandlerTests()
  {
    _sut = new UpdateSessionResourceCommandHandler(_sessionRepository, _unitOfWork, _validator);
  }

  private static Session CreateSessionWithVideoResource(out Guid resourceId)
  {
    var session = Session.Create(Guid.NewGuid(), "Test", "Speaker", DateTimeOffset.UtcNow.AddDays(7)).Value;
    var addResult = session.AddResource(SessionResourceType.Video, "https://youtube.com", null, "Video");
    resourceId = addResult.Value.Id;
    return session;
  }

  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccess()
  {
    // Arrange
    var session = CreateSessionWithVideoResource(out var resourceId);
    var command = new UpdateSessionResourceCommand(session.Id, resourceId, "https://youtube.com/new", null, "Updated");

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
    var command = new UpdateSessionResourceCommand(Guid.NewGuid(), Guid.NewGuid(), "https://youtube.com", null, null);

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
  public async Task HandleAsync_WhenResourceNotFound_ShouldReturnFailure()
  {
    // Arrange — session exists but resource ID doesn't
    var session = Session.Create(Guid.NewGuid(), "Test", "Speaker", DateTimeOffset.UtcNow.AddDays(7)).Value;
    var command = new UpdateSessionResourceCommand(session.Id, Guid.NewGuid(), "https://youtube.com", null, null);

    _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
        .Returns(new ValidationResult());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceNotFound);
  }
}

public class DeleteSessionResourceCommandHandlerTests
{
  private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
  private readonly DeleteSessionResourceCommandHandler _sut;

  public DeleteSessionResourceCommandHandlerTests()
  {
    _sut = new DeleteSessionResourceCommandHandler(_sessionRepository, _unitOfWork);
  }

  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccess()
  {
    // Arrange
    var session = Session.Create(Guid.NewGuid(), "Test", "Speaker", DateTimeOffset.UtcNow.AddDays(7)).Value;
    var addResult = session.AddResource(SessionResourceType.Video, "https://youtube.com", null, null);
    var resourceId = addResult.Value.Id;
    var command = new DeleteSessionResourceCommand(session.Id, resourceId);
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
    _sessionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
        .Returns((Session?)null);
    var command = new DeleteSessionResourceCommand(Guid.NewGuid(), Guid.NewGuid());

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.NotFound);
  }

  [Fact]
  public async Task HandleAsync_WhenResourceNotFound_ShouldReturnFailure()
  {
    // Arrange — no resources on session
    var session = Session.Create(Guid.NewGuid(), "Test", "Speaker", DateTimeOffset.UtcNow.AddDays(7)).Value;
    var command = new DeleteSessionResourceCommand(session.Id, Guid.NewGuid());
    _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
        .Returns(session);

    // Act
    var result = await _sut.HandleAsync(command);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceNotFound);
  }
}
