using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Resources.GetBySession;

namespace Gathering.Api.Endpoints.Sessions.Resources;

public sealed class GetBySession : IEndpoint
{
  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapGet("/sessions/{sessionId:guid}/resources", async (Guid sessionId, ISender sender, CancellationToken cancellationToken = default) =>
    {
      var query = new GetSessionResourcesQuery(sessionId);

      var result = await sender.Send(query, cancellationToken);

      if (result.IsFailure)
      {
        return Results.BadRequest(result.Error);
      }

      return Results.Ok(result.Value);
    })
    .WithTags(ApiTags.SessionResource);
  }
}
