using Gathering.Application.Abstractions;

namespace Gathering.Application.Communities.Create;

/// <summary>
/// Command to create a new community with optional image upload.
/// </summary>
/// <param name="Name">Community name (required)</param>
/// <param name="Description">Community description (required)</param>
/// <param name="ImageStream">Image file stream (optional)</param>
/// <param name="ImageFileName">Original image file name with extension (required if ImageStream provided)</param>
/// <param name="ImageContentType">MIME type of the image (required if ImageStream provided)</param>
public record CreateCommunityCommand(
    string Name,
    string Description,
    Stream? ImageStream = null,
    string? ImageFileName = null,
    string? ImageContentType = null) : ICommand;
