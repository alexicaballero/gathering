using FluentAssertions;
using Gathering.Domain.Sessions;

namespace Gathering.Domain.Tests.Sessions;

public class SessionResourceManagementTests
{
  private static Session CreateValidSession() =>
      Session.Create(
          Guid.NewGuid(),
          "Test Session",
          "Test Speaker",
          DateTimeOffset.UtcNow.AddDays(1)).Value;

  // ── AddResource ───────────────────────────────────────────────────────────

  [Fact]
  public void AddResource_WithValidVideoResource_ShouldReturnSuccessAndAddToList()
  {
    var session = CreateValidSession();

    var result = session.AddResource(SessionResourceType.Video, "https://youtube.com", null, "Video 1");

    result.IsSuccess.Should().BeTrue();
    session.Resources.Should().ContainSingle(r => r.Id == result.Value.Id);
  }

  [Fact]
  public void AddResource_WithValidNotesResource_ShouldReturnSuccess()
  {
    var session = CreateValidSession();

    var result = session.AddResource(SessionResourceType.Notes, null, "Session notes", "Notes 1");

    result.IsSuccess.Should().BeTrue();
    session.Resources.Should().ContainSingle();
  }

  [Fact]
  public void AddResource_WithInvalidType_ShouldReturnFailureAndNotAddToList()
  {
    var session = CreateValidSession();

    var result = session.AddResource((SessionResourceType)99, null, null, null);

    result.IsFailure.Should().BeTrue();
    session.Resources.Should().BeEmpty();
  }

  [Fact]
  public void AddResource_VideoWithoutUrl_ShouldReturnFailure()
  {
    var session = CreateValidSession();

    var result = session.AddResource(SessionResourceType.Video, null, null, null);

    result.IsFailure.Should().BeTrue();
    session.Resources.Should().BeEmpty();
  }

  [Fact]
  public void AddResource_MultipleResources_ShouldAllBePresent()
  {
    var session = CreateValidSession();

    session.AddResource(SessionResourceType.Video, "https://youtube.com", null, "Video");
    session.AddResource(SessionResourceType.Notes, null, "Some notes text", "Notes");
    session.AddResource(SessionResourceType.ExternalLink, "https://docs.com", null, "Docs");

    session.Resources.Should().HaveCount(3);
  }

  // ── UpdateResource ────────────────────────────────────────────────────────

  [Fact]
  public void UpdateResource_WhenResourceExists_ShouldReturnSuccess()
  {
    var session = CreateValidSession();
    var addResult = session.AddResource(SessionResourceType.Video, "https://youtube.com", null, "Old Title");
    var resourceId = addResult.Value.Id;

    var result = session.UpdateResource(resourceId, "https://youtube.com/new", null, "New Title");

    result.IsSuccess.Should().BeTrue();
    session.Resources.First(r => r.Id == resourceId).Title.Should().Be("New Title");
  }

  [Fact]
  public void UpdateResource_WhenResourceDoesNotExist_ShouldReturnFailure()
  {
    var session = CreateValidSession();

    var result = session.UpdateResource(Guid.NewGuid(), "https://youtube.com", null, null);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceNotFound);
  }

  // ── RemoveResource ────────────────────────────────────────────────────────

  [Fact]
  public void RemoveResource_WhenResourceExists_ShouldReturnSuccessAndRemoveFromList()
  {
    var session = CreateValidSession();
    var addResult = session.AddResource(SessionResourceType.Video, "https://youtube.com", null, null);
    var resourceId = addResult.Value.Id;

    var result = session.RemoveResource(resourceId);

    result.IsSuccess.Should().BeTrue();
    session.Resources.Should().BeEmpty();
  }

  [Fact]
  public void RemoveResource_WhenResourceDoesNotExist_ShouldReturnFailure()
  {
    var session = CreateValidSession();

    var result = session.RemoveResource(Guid.NewGuid());

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceNotFound);
  }

  [Fact]
  public void RemoveResource_OnlyRemovesTargetResource()
  {
    var session = CreateValidSession();
    var r1 = session.AddResource(SessionResourceType.Video, "https://youtube.com/1", null, null).Value;
    var r2 = session.AddResource(SessionResourceType.Video, "https://youtube.com/2", null, null).Value;

    session.RemoveResource(r1.Id);

    session.Resources.Should().ContainSingle(r => r.Id == r2.Id);
  }
}
