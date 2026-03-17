using FluentAssertions;
using Gathering.Application.Sessions.GetActive;
using Gathering.Application.Sessions.GetByCommunity;
using Gathering.Application.Sessions.GetById;
using Gathering.Application.Sessions.Resources.GetBySession;
using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;
using NSubstitute;

namespace Gathering.Application.Tests.Sessions;

public class SessionQueryHandlerTests
{
  // ── GetActiveSessionsQueryHandler ─────────────────────────────────────────

  public class GetActiveSessionsTests
  {
    private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
    private readonly GetActiveSessionsQueryHandler _sut;

    public GetActiveSessionsTests()
    {
      _sut = new GetActiveSessionsQueryHandler(_sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnMappedActiveSessions()
    {
      // Arrange
      var session = Session.Create(Guid.NewGuid(), "Active Session", "Speaker", DateTimeOffset.UtcNow.AddDays(1)).Value;
      _sessionRepository.GetActiveSessionsAsync(Arg.Any<CancellationToken>())
          .Returns(new List<Session> { session });

      // Act
      var result = await _sut.HandleAsync(new GetActiveSessionsQuery());

      // Assert
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().ContainSingle(r => r.Title == "Active Session");
    }

    [Fact]
    public async Task HandleAsync_WhenNoActiveSessions_ShouldReturnEmptyList()
    {
      // Arrange
      _sessionRepository.GetActiveSessionsAsync(Arg.Any<CancellationToken>())
          .Returns(new List<Session>());

      // Act
      var result = await _sut.HandleAsync(new GetActiveSessionsQuery());

      // Assert
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEmpty();
    }
  }

  // ── GetSessionsByCommunityQueryHandler ────────────────────────────────────

  public class GetSessionsByCommunityTests
  {
    private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
    private readonly ICommunityRepository _communityRepository = Substitute.For<ICommunityRepository>();
    private readonly GetSessionsByCommunityQueryHandler _sut;

    public GetSessionsByCommunityTests()
    {
      _sut = new GetSessionsByCommunityQueryHandler(_sessionRepository, _communityRepository);
    }

    [Fact]
    public async Task HandleAsync_WhenCommunityExists_ShouldReturnSessions()
    {
      // Arrange
      var communityId = Guid.NewGuid();
      var session = Session.Create(communityId, "Community Session", "Speaker", DateTimeOffset.UtcNow.AddDays(1)).Value;
      _communityRepository.ExistsAsync(communityId, Arg.Any<CancellationToken>())
          .Returns(true);
      _sessionRepository.GetByCommunityIdAsync(communityId, Arg.Any<CancellationToken>())
          .Returns(new List<Session> { session });

      // Act
      var result = await _sut.HandleAsync(new GetSessionsByCommunityQuery(communityId));

      // Assert
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().ContainSingle(r => r.CommunityId == communityId);
    }

    [Fact]
    public async Task HandleAsync_WhenCommunityNotFound_ShouldReturnFailure()
    {
      // Arrange
      var communityId = Guid.NewGuid();
      _communityRepository.ExistsAsync(communityId, Arg.Any<CancellationToken>())
          .Returns(false);

      // Act
      var result = await _sut.HandleAsync(new GetSessionsByCommunityQuery(communityId));

      // Assert
      result.IsFailure.Should().BeTrue();
      result.Error.Should().Be(CommunityError.NotFound);
    }
  }

  // ── GetSessionByIdQueryHandler ────────────────────────────────────────────

  public class GetSessionByIdTests
  {
    private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
    private readonly GetSessionByIdQueryHandler _sut;

    public GetSessionByIdTests()
    {
      _sut = new GetSessionByIdQueryHandler(_sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WhenSessionExists_ShouldReturnMappedResponse()
    {
      // Arrange
      var session = Session.Create(Guid.NewGuid(), "My Session", "Speaker", DateTimeOffset.UtcNow.AddDays(1)).Value;
      _sessionRepository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
          .Returns(session);

      // Act
      var result = await _sut.HandleAsync(new GetSessionByIdQuery(session.Id));

      // Assert
      result.IsSuccess.Should().BeTrue();
      result.Value.Id.Should().Be(session.Id);
      result.Value.Title.Should().Be("My Session");
    }

    [Fact]
    public async Task HandleAsync_WhenSessionNotFound_ShouldReturnFailure()
    {
      // Arrange
      _sessionRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
          .Returns((Session?)null);

      // Act
      var result = await _sut.HandleAsync(new GetSessionByIdQuery(Guid.NewGuid()));

      // Assert
      result.IsFailure.Should().BeTrue();
      result.Error.Should().Be(SessionError.NotFound);
    }
  }

  // ── GetSessionResourcesQueryHandler ──────────────────────────────────────

  public class GetSessionResourcesTests
  {
    private readonly ISessionRepository _sessionRepository = Substitute.For<ISessionRepository>();
    private readonly GetSessionResourcesQueryHandler _sut;

    public GetSessionResourcesTests()
    {
      _sut = new GetSessionResourcesQueryHandler(_sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WhenSessionExists_ShouldReturnResources()
    {
      // Arrange
      var sessionId = Guid.NewGuid();
      _sessionRepository.ExistsAsync(sessionId, Arg.Any<CancellationToken>())
          .Returns(true);
      _sessionRepository.GetResourcesBySessionIdAsync(sessionId, Arg.Any<CancellationToken>())
          .Returns(new List<SessionResource>());

      // Act
      var result = await _sut.HandleAsync(new GetSessionResourcesQuery(sessionId));

      // Assert
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WhenSessionNotFound_ShouldReturnFailure()
    {
      // Arrange
      var sessionId = Guid.NewGuid();
      _sessionRepository.ExistsAsync(sessionId, Arg.Any<CancellationToken>())
          .Returns(false);

      // Act
      var result = await _sut.HandleAsync(new GetSessionResourcesQuery(sessionId));

      // Assert
      result.IsFailure.Should().BeTrue();
      result.Error.Should().Be(SessionError.NotFound);
    }
  }
}
