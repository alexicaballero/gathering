using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Resources.Delete;

namespace Gathering.Api.Endpoints.Sessions.Resources;

public sealed class Delete : IEndpoint
{
  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapDelete("/sessions/{sessionId:guid}/resources/{resourceId:guid}", async (
        Guid sessionId,
        Guid resourceId,
        ISender sender,
        CancellationToken cancellationToken = default) =>
    {
      var command = new DeleteSessionResourceCommand(sessionId, resourceId);

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
