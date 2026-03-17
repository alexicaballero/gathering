using FluentAssertions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Domain.Tests.Sessions;

public class SessionTests
{
  private static readonly Guid ValidCommunityId = Guid.NewGuid();
  private const string ValidTitle = "Introduction to xUnit";
  private const string ValidSpeaker = "John Doe";
  private static readonly DateTimeOffset FutureDate = DateTimeOffset.UtcNow.AddDays(7);

  // ── Create ───────────────────────────────────────────────────────────────

  [Fact]
  public void Create_WithValidArguments_ShouldReturnSuccess()
  {
    var result = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate);

    result.IsSuccess.Should().BeTrue();
    result.Value.CommunityId.Should().Be(ValidCommunityId);
    result.Value.Title.Should().Be(ValidTitle);
    result.Value.Speaker.Should().Be(ValidSpeaker);
    result.Value.ScheduledAt.Should().Be(FutureDate);
    result.Value.Status.Should().Be(SessionStatus.Scheduled);
    result.Value.Id.Should().NotBeEmpty();
  }

  [Fact]
  public void Create_WithDescription_ShouldSetDescriptionProperty()
  {
    var result = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate, "A great session");

    result.IsSuccess.Should().BeTrue();
    result.Value.Description.Should().Be("A great session");
  }

  [Fact]
  public void Create_ShouldRaiseSessionCreatedDomainEvent()
  {
    var result = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate);

    result.Value.DomainEvents.Should().ContainSingle()
        .Which.Should().BeOfType<SessionCreatedDomainEvent>()
        .Which.SessionId.Should().Be(result.Value.Id);
  }

  [Fact]
  public void Create_WithEmptyCommunityId_ShouldReturnFailure()
  {
    var result = Session.Create(Guid.Empty, ValidTitle, ValidSpeaker, FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.CommunityInvalid);
  }

  [Fact]
  public void Create_WithEmptyTitle_ShouldReturnFailure()
  {
    var result = Session.Create(ValidCommunityId, "", ValidSpeaker, FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.TitleEmpty);
  }

  [Fact]
  public void Create_WithWhitespaceTitle_ShouldReturnFailure()
  {
    var result = Session.Create(ValidCommunityId, "   ", ValidSpeaker, FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.TitleEmpty);
  }

  [Fact]
  public void Create_WithTitleExceeding200Chars_ShouldReturnFailure()
  {
    var result = Session.Create(ValidCommunityId, new string('A', 201), ValidSpeaker, FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.TitleTooLong);
  }

  [Fact]
  public void Create_WithTitleExactly200Chars_ShouldReturnSuccess()
  {
    var result = Session.Create(ValidCommunityId, new string('A', 200), ValidSpeaker, FutureDate);

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void Create_WithDescriptionExceeding1000Chars_ShouldReturnFailure()
  {
    var result = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate, new string('A', 1001));

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.DescriptionTooLong);
  }

  [Fact]
  public void Create_WithEmptySpeaker_ShouldReturnFailure()
  {
    var result = Session.Create(ValidCommunityId, ValidTitle, "", FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.SpeakerEmpty);
  }

  [Fact]
  public void Create_WithPastScheduledAt_ShouldReturnFailure()
  {
    var pastDate = DateTimeOffset.UtcNow.AddDays(-1);

    var result = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, pastDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ScheduleInvalid);
  }

  [Fact]
  public void Create_WithScheduledAtEqualToNow_ShouldReturnFailure()
  {
    // scheduledAt <= UtcNow  means equal is also invalid
    var result = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, DateTimeOffset.UtcNow);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ScheduleInvalid);
  }

  // ── CreateCompleted (internal) ────────────────────────────────────────────

  [Fact]
  public void CreateCompleted_WithValidArguments_ShouldReturnSuccessWithCompletedStatus()
  {
    var pastDate = DateTimeOffset.UtcNow.AddDays(-1);

    var result = Session.CreateCompleted(ValidCommunityId, ValidTitle, ValidSpeaker, pastDate);

    result.IsSuccess.Should().BeTrue();
    result.Value.Status.Should().Be(SessionStatus.Completed);
  }

  [Fact]
  public void CreateCompleted_ShouldNotRaiseDomainEvent()
  {
    var result = Session.CreateCompleted(ValidCommunityId, ValidTitle, ValidSpeaker, DateTimeOffset.UtcNow.AddDays(-1));

    result.Value.DomainEvents.Should().BeEmpty();
  }

  [Fact]
  public void CreateCompleted_WithEmptyCommunityId_ShouldReturnFailure()
  {
    var result = Session.CreateCompleted(Guid.Empty, ValidTitle, ValidSpeaker, DateTimeOffset.UtcNow.AddDays(-1));

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.CommunityInvalid);
  }

  [Fact]
  public void CreateCompleted_WithEmptyTitle_ShouldReturnFailure()
  {
    var result = Session.CreateCompleted(ValidCommunityId, "", ValidSpeaker, DateTimeOffset.UtcNow.AddDays(-1));

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.TitleEmpty);
  }

  [Fact]
  public void CreateCompleted_WithEmptySpeaker_ShouldReturnFailure()
  {
    var result = Session.CreateCompleted(ValidCommunityId, ValidTitle, "", DateTimeOffset.UtcNow.AddDays(-1));

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.SpeakerEmpty);
  }

  // ── Update ────────────────────────────────────────────────────────────────

  [Fact]
  public void Update_WithValidArguments_ShouldReturnSuccess()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;

    var result = session.Update("New Title", "New Speaker", FutureDate.AddDays(1));

    result.IsSuccess.Should().BeTrue();
    session.Title.Should().Be("New Title");
    session.Speaker.Should().Be("New Speaker");
  }

  [Fact]
  public void Update_WithEmptyTitle_ShouldReturnFailure()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;

    var result = session.Update("", ValidSpeaker, FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.TitleEmpty);
  }

  [Fact]
  public void Update_WithTitleExceeding200Chars_ShouldReturnFailure()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;

    var result = session.Update(new string('A', 201), ValidSpeaker, FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.TitleTooLong);
  }

  [Fact]
  public void Update_WithDescriptionExceeding1000Chars_ShouldReturnFailure()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;

    var result = session.Update(ValidTitle, ValidSpeaker, FutureDate, new string('A', 1001));

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.DescriptionTooLong);
  }

  [Fact]
  public void Update_WithEmptySpeaker_ShouldReturnFailure()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;

    var result = session.Update(ValidTitle, "", FutureDate);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.SpeakerEmpty);
  }

  // ── UpdateStatus ──────────────────────────────────────────────────────────

  [Fact]
  public void UpdateStatus_ScheduledToCompleted_ShouldReturnSuccess()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;

    var result = session.UpdateStatus(SessionStatus.Completed);

    result.IsSuccess.Should().BeTrue();
    session.Status.Should().Be(SessionStatus.Completed);
  }

  [Fact]
  public void UpdateStatus_ScheduledToCanceled_ShouldReturnSuccess()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;

    var result = session.UpdateStatus(SessionStatus.Canceled);

    result.IsSuccess.Should().BeTrue();
    session.Status.Should().Be(SessionStatus.Canceled);
  }

  [Fact]
  public void UpdateStatus_CanceledToScheduled_ShouldReturnFailure()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;
    session.UpdateStatus(SessionStatus.Canceled);

    var result = session.UpdateStatus(SessionStatus.Scheduled);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.InvalidStatusTransition);
  }

  [Fact]
  public void UpdateStatus_CompletedToScheduled_ShouldReturnFailure()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;
    session.UpdateStatus(SessionStatus.Completed);

    var result = session.UpdateStatus(SessionStatus.Scheduled);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.InvalidStatusTransition);
  }

  [Fact]
  public void UpdateStatus_CompletedToCanceled_ShouldReturnSuccess()
  {
    var session = Session.Create(ValidCommunityId, ValidTitle, ValidSpeaker, FutureDate).Value;
    session.UpdateStatus(SessionStatus.Completed);

    var result = session.UpdateStatus(SessionStatus.Canceled);

    result.IsSuccess.Should().BeTrue();
  }
}
