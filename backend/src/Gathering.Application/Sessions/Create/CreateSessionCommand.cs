using Gathering.Application.Abstractions;

namespace Gathering.Application.Sessions.Create;

/// <summary>
/// Command to create a new session with optional image upload.
/// </summary>
/// <param name="CommunityId">ID of the community this session belongs to (required)</param>
/// <param name="Title">Session title (required)</param>
/// <param name="Description">Session description (required)</param>
/// <param name="Speaker">Session speaker name (required)</param>
/// <param name="Schedule">Session scheduled date and time (required)</param>
/// <param name="ImageStream">Image file stream (optional)</param>
/// <param name="ImageFileName">Original image file name with extension (required if ImageStream provided)</param>
/// <param name="ImageContentType">MIME type of the image (required if ImageStream provided)</param>
public sealed record CreateSessionCommand(
    Guid CommunityId,
    string Title,
    string Description,
    string Speaker,
    DateTime Schedule,
    Stream? ImageStream = null,
    string? ImageFileName = null,
    string? ImageContentType = null) : ICommand<Guid>;
