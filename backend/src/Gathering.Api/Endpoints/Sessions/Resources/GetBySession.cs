using Gathering.Api.Extensions;
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

      return result.Match(Results.Ok, CustomResults.Problem);
    })
    .WithTags(ApiTags.SessionResource);
  }
}
