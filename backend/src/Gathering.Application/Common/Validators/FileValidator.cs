using Gathering.SharedKernel;

namespace Gathering.Application.Common.Validators;

/// <summary>
/// Validates image files for size, format, and extension.
/// </summary>
public static class FileValidator
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    ];

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp"
    ];

    /// <summary>
    /// Validates an image stream for size, extension, and content type.
    /// </summary>
    /// <param name="fileStream">The file stream to validate</param>
    /// <param name="fileName">The original file name</param>
    /// <param name="contentType">The MIME type of the file</param>
    /// <returns>Success result if valid, Failure result with error details if invalid</returns>
    public static Result ValidateImageFile(Stream fileStream, string fileName, string contentType)
    {
        // Validate content type
        if (!AllowedContentTypes.Contains(contentType.ToLower()))
        {
            return Result.Failure(ImageValidationError.InvalidContentType);
        }

        // Validate extension
        var extension = Path.GetExtension(fileName).ToLower();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return Result.Failure(ImageValidationError.InvalidExtension);
        }

        // Validate file size
        if (fileStream.Length > MaxFileSizeBytes)
        {
            return Result.Failure(ImageValidationError.FileTooLarge);
        }

        return Result.Success();
    }
}

/// <summary>
/// Error definitions for image validation failures.
/// </summary>
public static class ImageValidationError
{
    public static readonly Error InvalidContentType = new(
        "ImageValidation.InvalidContentType",
        "The file type is not supported. Allowed types: JPEG, PNG, GIF, WebP.",
        ErrorType.Validation);

    public static readonly Error InvalidExtension = new(
        "ImageValidation.InvalidExtension",
        "The file extension is not supported. Allowed extensions: .jpg, .jpeg, .png, .gif, .webp.",
        ErrorType.Validation);

    public static readonly Error FileTooLarge = new(
        "ImageValidation.FileTooLarge",
        "The file size exceeds the maximum allowed size of 10 MB.",
        ErrorType.Validation);
}
