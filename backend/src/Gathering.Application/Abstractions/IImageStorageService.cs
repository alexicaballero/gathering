namespace Gathering.Application.Abstractions;

using Gathering.SharedKernel;

/// <summary>
/// Defines contract for image storage operations (upload, delete).
/// Implementations handle cloud storage platforms like Azure Blob Storage.
/// </summary>
public interface IImageStorageService
{
  /// <summary>
  /// Uploads an image to storage and returns the blob URL.
  /// </summary>
  /// <param name="imageStream">The image file stream to upload</param>
  /// <param name="fileName">Original file name (used for extension extraction)</param>
  /// <param name="contentType">MIME type of the image (e.g., "image/jpeg")</param>
  /// <param name="entityType">Entity type for blob naming (e.g., "communities", "sessions")</param>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>Result containing the blob URL on success, or an error</returns>
  Task<Result<string>> UploadImageAsync(
      Stream imageStream,
      string fileName,
      string contentType,
      string entityType,
      CancellationToken cancellationToken = default);

  /// <summary>
  /// Deletes an image from storage by its blob URL.
  /// </summary>
  /// <param name="imageUrl">The full blob URL to delete</param>
  /// <param name="cancellationToken">Cancellation token</param>
  /// <returns>Result indicating success or error</returns>
  Task<Result> DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}
