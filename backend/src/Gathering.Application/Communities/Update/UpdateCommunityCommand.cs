using Gathering.Application.Abstractions;

namespace Gathering.Application.Communities.Update;

/// <summary>
/// Command to update an existing community with optional new image upload.
/// </summary>
/// <param name="CommunityId">ID of the community to update</param>
/// <param name="Name">Updated community name (required)</param>
/// <param name="Description">Updated community description (required)</param>
/// <param name="ImageStream">New image file stream (optional)</param>
/// <param name="ImageFileName">Original image file name with extension (required if ImageStream provided)</param>
/// <param name="ImageContentType">MIME type of the new image (required if ImageStream provided)</param>
public sealed record UpdateCommunityCommand(
    Guid CommunityId,
    string Name,
    string Description,
    Stream? ImageStream = null,
    string? ImageFileName = null,
    string? ImageContentType = null) : ICommand;
