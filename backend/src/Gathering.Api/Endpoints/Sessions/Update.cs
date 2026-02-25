using Gathering.Api.Extensions;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Update;
using Gathering.Domain.Sessions;
using Microsoft.AspNetCore.Mvc;
using Gathering.Infrastructure;
using Gathering.SharedKernel;

namespace Gathering.Api.Endpoints.Sessions;

public class Update : IEndpoint
{
  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapPut("/sessions/{id:guid}", Handler)
      .WithTags(ApiTags.Session)
      .Accepts<IFormCollection>("multipart/form-data")
      .Produces(StatusCodes.Status204NoContent)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status404NotFound)
      .WithName("UpdateSession")
      .DisableAntiforgery(); // Disable CSRF protection for API endpoints
  }

  private static async Task<IResult> Handler(
    Guid id,
    [FromForm] string title,
    [FromForm] string description,
    [FromForm] string speaker,
    [FromForm] DateTime schedule,
    [FromForm] SessionState state,
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

    var updateSessionCommand = new UpdateSessionCommand(
      id,
      title,
      description,
      speaker,
      schedule,
      state,
      imageStream,
      imageFileName,
      imageContentType);

    Result<Guid> result = await sender.Send(updateSessionCommand, cancellationToken);

    if (imageStream is not null)
    {
      await imageStream.DisposeAsync();
    }

    return result.Match(Results.Ok, CustomResults.Problem);
  }
}