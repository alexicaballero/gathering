using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Resources.Update;

namespace Gathering.Api.Endpoints.Sessions.Resources;

public sealed class Update : IEndpoint
{
  public sealed record Request(string? Url, string? Notes, string? Title);

  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapPut("/sessions/{sessionId:guid}/resources/{resourceId:guid}", async (
        Guid sessionId,
        Guid resourceId,
        Request request,
        ISender sender,
        CancellationToken cancellationToken = default) =>
    {
      var command = new UpdateSessionResourceCommand(
              sessionId,
              resourceId,
              request.Url,
              request.Notes,
              request.Title);

      var result = await sender.Send(command, cancellationToken);

      if (result.IsFailure)
      {
        return Results.BadRequest(result.Error);
      }

      return Results.NoContent();
    })
    .WithTags(ApiTags.SessionResource);
  }
}
