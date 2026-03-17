using FluentAssertions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Domain.Tests.Sessions;

public class SessionResourceTests
{
  private static readonly Guid ValidSessionId = Guid.NewGuid();

  // ── Create Video ─────────────────────────────────────────────────────────

  [Fact]
  public void Create_VideoWithValidUrl_ShouldReturnSuccess()
  {
    var result = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Video,
        "https://youtube.com/watch?v=abc",
        null,
        "Intro Video");

    result.IsSuccess.Should().BeTrue();
    result.Value.Type.Should().Be(SessionResourceType.Video);
    result.Value.Url.Should().Be("https://youtube.com/watch?v=abc");
    result.Value.Notes.Should().BeNull();
    result.Value.Title.Should().Be("Intro Video");
    result.Value.SessionId.Should().Be(ValidSessionId);
  }

  [Fact]
  public void Create_VideoWithoutUrl_ShouldReturnFailure()
  {
    var result = SessionResource.Create(ValidSessionId, SessionResourceType.Video, null, null, "Video");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceUrlEmpty);
  }

  [Fact]
  public void Create_VideoWithEmptyUrl_ShouldReturnFailure()
  {
    var result = SessionResource.Create(ValidSessionId, SessionResourceType.Video, "", null, "Video");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceUrlEmpty);
  }

  [Fact]
  public void Create_VideoWithUrlExceeding2000Chars_ShouldReturnFailure()
  {
    var longUrl = "https://example.com/" + new string('a', 1985);

    var result = SessionResource.Create(ValidSessionId, SessionResourceType.Video, longUrl, null, null);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceUrlTooLong);
  }

  // ── Create Notes ─────────────────────────────────────────────────────────

  [Fact]
  public void Create_NotesWithValidNotes_ShouldReturnSuccess()
  {
    var result = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Notes,
        null,
        "These are the session notes.",
        "My Notes");

    result.IsSuccess.Should().BeTrue();
    result.Value.Notes.Should().Be("These are the session notes.");
    result.Value.Url.Should().BeNull();
  }

  [Fact]
  public void Create_NotesWithoutNotes_ShouldReturnFailure()
  {
    var result = SessionResource.Create(ValidSessionId, SessionResourceType.Notes, null, null, "Title");

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceNotesEmpty);
  }

  [Fact]
  public void Create_NotesWithNotesExceeding4000Chars_ShouldReturnFailure()
  {
    var longNotes = new string('A', 4001);

    var result = SessionResource.Create(ValidSessionId, SessionResourceType.Notes, null, longNotes, null);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceNotesTooLong);
  }

  // ── Create ExternalLink ───────────────────────────────────────────────────

  [Fact]
  public void Create_ExternalLinkWithValidUrl_ShouldReturnSuccess()
  {
    var result = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.ExternalLink,
        "https://docs.microsoft.com",
        null,
        "MS Docs");

    result.IsSuccess.Should().BeTrue();
    result.Value.Url.Should().Be("https://docs.microsoft.com");
  }

  [Fact]
  public void Create_ExternalLinkWithoutUrl_ShouldReturnFailure()
  {
    var result = SessionResource.Create(ValidSessionId, SessionResourceType.ExternalLink, null, null, null);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceUrlEmpty);
  }

  // ── Title validation ──────────────────────────────────────────────────────

  [Fact]
  public void Create_WithTitleExceeding200Chars_ShouldReturnFailure()
  {
    var result = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Video,
        "https://youtube.com",
        null,
        new string('A', 201));

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceTitleTooLong);
  }

  [Fact]
  public void Create_WithTitleExactly200Chars_ShouldReturnSuccess()
  {
    var result = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Video,
        "https://youtube.com",
        null,
        new string('A', 200));

    result.IsSuccess.Should().BeTrue();
  }

  // ── Invalid type ──────────────────────────────────────────────────────────

  [Fact]
  public void Create_WithInvalidType_ShouldReturnFailure()
  {
    var result = SessionResource.Create(ValidSessionId, (SessionResourceType)99, null, null, null);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceTypeInvalid);
  }

  // ── Notes type does not store URL ─────────────────────────────────────────

  [Fact]
  public void Create_NotesType_ShouldNotStoreUrlEvenIfProvided()
  {
    var result = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Notes,
        "https://example.com",
        "Some notes content",
        null);

    result.IsSuccess.Should().BeTrue();
    result.Value.Url.Should().BeNull();
  }

  // ── Update ────────────────────────────────────────────────────────────────

  [Fact]
  public void Update_VideoWithNewValidUrl_ShouldReturnSuccess()
  {
    var resource = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Video,
        "https://youtube.com/old",
        null,
        null).Value;

    var result = resource.Update("https://youtube.com/new", null, "Updated Title");

    result.IsSuccess.Should().BeTrue();
    resource.Url.Should().Be("https://youtube.com/new");
    resource.Title.Should().Be("Updated Title");
  }

  [Fact]
  public void Update_VideoWithoutUrl_ShouldReturnFailure()
  {
    var resource = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Video,
        "https://youtube.com",
        null,
        null).Value;

    var result = resource.Update(null, null, null);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(SessionError.ResourceUrlEmpty);
  }

  [Fact]
  public void Update_NotesWithNewNotes_ShouldReturnSuccess()
  {
    var resource = SessionResource.Create(
        ValidSessionId,
        SessionResourceType.Notes,
        null,
        "Original notes",
        null).Value;

    var result = resource.Update(null, "Updated notes content", "Updated Title");

    result.IsSuccess.Should().BeTrue();
    resource.Notes.Should().Be("Updated notes content");
  }
}
