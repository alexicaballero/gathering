using Gathering.Application.Abstractions;
using Gathering.Domain.Sessions;

namespace Gathering.Application.Sessions.Update;

/// <summary>
/// Command to update an existing session with optional new image upload.
/// </summary>
/// <param name="SessionId">ID of the session to update</param>
/// <param name="Title">Updated session title (required)</param>
/// <param name="Speaker">Updated speaker name (required)</param>
/// <param name="ScheduledAt">Updated schedule date and time (required)</param>
/// <param name="Status">Updated session status (required)</param>
/// <param name="Description">Updated session description (optional)</param>
/// <param name="ImageStream">New image file stream (optional)</param>
/// <param name="ImageFileName">Original image file name with extension (required if ImageStream provided)</param>
/// <param name="ImageContentType">MIME type of the new image (required if ImageStream provided)</param>
public sealed record UpdateSessionCommand(
    Guid SessionId,
    string Title,
    string Speaker,
    DateTimeOffset ScheduledAt,
    SessionStatus Status,
    string? Description = null,
    Stream? ImageStream = null,
    string? ImageFileName = null,
    string? ImageContentType = null) : ICommand<Guid>;