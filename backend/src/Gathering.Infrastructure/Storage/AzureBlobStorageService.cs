using Gathering.Application.Abstractions;
using Gathering.SharedKernel;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Gathering.Infrastructure.Storage;

/// <summary>
/// Implementation of image storage using Azure Blob Storage.
/// Handles uploading and deleting images from Azure containers.
/// </summary>
internal sealed class AzureBlobStorageService(BlobServiceClient blobServiceClient) : IImageStorageService
{
  private const string ContainerName = "images";

  /// <summary>
  /// Uploads an image to Azure Blob Storage and returns the blob URL.
  /// Creates container if it doesn't exist on first use.
  /// </summary>
  public async Task<Result<string>> UploadImageAsync(
      Stream imageStream,
      string fileName,
      string contentType,
      string entityType,
      CancellationToken cancellationToken = default)
  {
    try
    {
      // Ensure container exists
      var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
      await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

      // Generate unique blob name: entities/guid.extension
      var extension = Path.GetExtension(fileName).ToLower();
      var blobName = $"{entityType}/{Guid.NewGuid()}{extension}";
      var blobClient = containerClient.GetBlobClient(blobName);

      // Reset stream to beginning
      imageStream.Seek(0, SeekOrigin.Begin);

      // Upload blob with content type metadata
      var uploadOptions = new BlobUploadOptions
      {
        HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
      };

      await blobClient.UploadAsync(imageStream, uploadOptions, cancellationToken);

      // Return the blob URI
      return Result.Success(blobClient.Uri.AbsoluteUri);
    }
    catch (Exception ex)
    {
      return Result.Failure<string>(ImageStorageError.UploadFailed(ex.Message));
    }
  }

  /// <summary>
  /// Deletes an image from Azure Blob Storage by its blob URL.
  /// Silently succeeds if blob doesn't exist.
  /// </summary>
  public async Task<Result> DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var uri = new Uri(imageUrl);
      var blobClient = new BlobClient(uri);

      // Delete blob, ignoring 404 errors (blob already deleted)
      await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

      return Result.Success();
    }
    catch (Exception ex)
    {
      // Log but don't fail - deletion errors shouldn't block operations
      // In production, you'd log this to your logging service
      return Result.Failure(ImageStorageError.DeleteFailed(ex.Message));
    }
  }
}

/// <summary>
/// Error definitions for image storage operations.
/// </summary>
public static class ImageStorageError
{
  public static Error UploadFailed(string details) => new(
      "ImageStorage.UploadFailed",
      $"Failed to upload image to storage: {details}",
      ErrorType.Problem);

  public static Error DeleteFailed(string details) => new(
      "ImageStorage.DeleteFailed",
      $"Failed to delete image from storage: {details}",
      ErrorType.Problem);
}
