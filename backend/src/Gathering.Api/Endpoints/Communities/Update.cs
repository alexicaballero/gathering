using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Update;
using Gathering.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace Gathering.Api.Endpoints.Communities;

public sealed class Update : IEndpoint
{
  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapPut("/communities/{id:guid}", Handler)
      .WithTags(ApiTags.Community)
      .Accepts<IFormCollection>("multipart/form-data")
      .Produces(StatusCodes.Status204NoContent)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound)
      .WithName("UpdateCommunity")
      .DisableAntiforgery(); // Disable CSRF protection for API endpoints
  }

  private static async Task<IResult> Handler(
    Guid id,
    [FromForm] string name,
    [FromForm] string description,
    [FromForm(Name = "image")] IFormFile? image,
    ISender sender,
    CancellationToken cancellationToken = default)
  {
    Stream? imageStream = null;
    string? imageFileName = null;
    string? imageContentType = null;

    if (image is not null)
    {
      imageStream = image.OpenReadStream();
      imageFileName = image.FileName;
      imageContentType = image.ContentType;
    }

    var updateCommunityCommand = new UpdateCommunityCommand(
      id,
      name,
      description,
      imageStream,
      imageFileName,
      imageContentType);

    Result<Guid> result = await sender.Send(updateCommunityCommand, cancellationToken);

    if (imageStream is not null)
    {
      await imageStream.DisposeAsync();
    }

    return result.Match(Results.Ok, CustomResults.Problem);
  }
}