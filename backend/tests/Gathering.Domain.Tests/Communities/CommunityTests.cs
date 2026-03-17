using FluentAssertions;
using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Domain.Tests.Communities;

public class CommunityTests
{
  private const string ValidName = "Test Community";
  private const string ValidDescription = "A valid description for the community.";

  // ── Create ───────────────────────────────────────────────────────────────

  [Fact]
  public void Create_WithValidArguments_ShouldReturnSuccess()
  {
    var result = Community.Create(ValidName, ValidDescription);

    result.IsSuccess.Should().BeTrue();
    result.Value.Name.Should().Be(ValidName);
    result.Value.Description.Should().Be(ValidDescription);
    result.Value.Image.Should().BeNull();
    result.Value.Id.Should().NotBeEmpty();
  }

  [Fact]
  public void Create_WithImage_ShouldSetImageProperty()
  {
    var result = Community.Create(ValidName, ValidDescription, "https://example.com/image.jpg");

    result.IsSuccess.Should().BeTrue();
    result.Value.Image.Should().Be("https://example.com/image.jpg");
  }

  [Fact]
  public void Create_WithEmptyName_ShouldReturnFailure()
  {
    var result = Community.Create("", ValidDescription);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NameEmpty);
  }

  [Fact]
  public void Create_WithWhitespaceName_ShouldReturnFailure()
  {
    var result = Community.Create("   ", ValidDescription);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NameEmpty);
  }

  [Fact]
  public void Create_WithNameExceeding200Chars_ShouldReturnFailure()
  {
    var longName = new string('A', 201);

    var result = Community.Create(longName, ValidDescription);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NameTooLong);
  }

  [Fact]
  public void Create_WithNameExactly200Chars_ShouldReturnSuccess()
  {
    var exactName = new string('A', 200);

    var result = Community.Create(exactName, ValidDescription);

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void Create_WithEmptyDescription_ShouldReturnFailure()
  {
    var result = Community.Create(ValidName, "");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.DescriptionEmpty);
  }

  [Fact]
  public void Create_WithWhitespaceDescription_ShouldReturnFailure()
  {
    var result = Community.Create(ValidName, "   ");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.DescriptionEmpty);
  }

  [Fact]
  public void Create_WithDescriptionExceeding1000Chars_ShouldReturnFailure()
  {
    var longDescription = new string('A', 1001);

    var result = Community.Create(ValidName, longDescription);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.DescriptionTooLong);
  }

  [Fact]
  public void Create_WithDescriptionExactly1000Chars_ShouldReturnSuccess()
  {
    var exactDescription = new string('A', 1000);

    var result = Community.Create(ValidName, exactDescription);

    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void Create_ShouldRaiseCommunityCreatedDomainEvent()
  {
    var result = Community.Create(ValidName, ValidDescription);

    result.IsSuccess.Should().BeTrue();
    result.Value.DomainEvents.Should().ContainSingle()
        .Which.Should().BeOfType<CommunityCreatedDomainEvent>()
        .Which.CommunityId.Should().Be(result.Value.Id);
  }

  // ── Update ────────────────────────────────────────────────────────────────

  [Fact]
  public void Update_WithValidArguments_ShouldReturnSuccess()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;

    var result = community.Update("New Name", "New description that is valid.");

    result.IsSuccess.Should().BeTrue();
    community.Name.Should().Be("New Name");
    community.Description.Should().Be("New description that is valid.");
  }

  [Fact]
  public void Update_WithNewImage_ShouldUpdateImageProperty()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;

    community.Update(ValidName, ValidDescription, "https://blob.core.windows.net/img.jpg");

    community.Image.Should().Be("https://blob.core.windows.net/img.jpg");
  }

  [Fact]
  public void Update_WithEmptyName_ShouldReturnFailure()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;

    var result = community.Update("", ValidDescription);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NameEmpty);
  }

  [Fact]
  public void Update_WithNameExceeding200Chars_ShouldReturnFailure()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;

    var result = community.Update(new string('A', 201), ValidDescription);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.NameTooLong);
  }

  [Fact]
  public void Update_WithEmptyDescription_ShouldReturnFailure()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;

    var result = community.Update(ValidName, "");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.DescriptionEmpty);
  }

  [Fact]
  public void Update_WithDescriptionExceeding1000Chars_ShouldReturnFailure()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;

    var result = community.Update(ValidName, new string('A', 1001));

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.DescriptionTooLong);
  }

  // ── AddSession ────────────────────────────────────────────────────────────

  [Fact]
  public void AddSession_WithValidSession_ShouldReturnSuccess()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;
    var session = Session.Create(
        community.Id,
        "Session Title",
        "Speaker Name",
        DateTimeOffset.UtcNow.AddDays(1)).Value;

    var result = community.AddSession(session);

    result.IsSuccess.Should().BeTrue();
    community.Sessions.Should().ContainSingle(s => s.Id == session.Id);
  }

  [Fact]
  public void AddSession_WhenSessionBelongsToDifferentCommunity_ShouldReturnFailure()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;
    var otherCommunityId = Guid.NewGuid();
    var session = Session.Create(
        otherCommunityId,
        "Session Title",
        "Speaker Name",
        DateTimeOffset.UtcNow.AddDays(1)).Value;

    var result = community.AddSession(session);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.SessionNotBelongsToCommunity);
  }

  [Fact]
  public void AddSession_WhenSessionAlreadyExists_ShouldReturnFailure()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;
    var session = Session.Create(
        community.Id,
        "Session Title",
        "Speaker Name",
        DateTimeOffset.UtcNow.AddDays(1)).Value;
    community.AddSession(session);

    var result = community.AddSession(session);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.SessionAlreadyExists);
  }

  // ── RemoveSession ─────────────────────────────────────────────────────────

  [Fact]
  public void RemoveSession_WhenSessionExists_ShouldReturnSuccess()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;
    var session = Session.Create(
        community.Id,
        "Session Title",
        "Speaker Name",
        DateTimeOffset.UtcNow.AddDays(1)).Value;
    community.AddSession(session);

    var result = community.RemoveSession(session.Id);

    result.IsSuccess.Should().BeTrue();
    community.Sessions.Should().BeEmpty();
  }

  [Fact]
  public void RemoveSession_WhenSessionDoesNotExist_ShouldReturnFailure()
  {
    var community = Community.Create(ValidName, ValidDescription).Value;

    var result = community.RemoveSession(Guid.NewGuid());

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(CommunityError.SessionNotFound);
  }
}
