using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.GetActive;

namespace Gathering.Api.Endpoints.Sessions;

public class GetActive : IEndpoint
{
  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapGet("/sessions/active", async (ISender sender, CancellationToken cancellationToken = default) =>
    {
      var query = new GetActiveSessionsQuery();

      var result = await sender.Send(query, cancellationToken);

      return Results.Ok(result.Value);
    })
    .WithTags(ApiTags.Session);
  }
}
