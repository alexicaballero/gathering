using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.GetActive;

namespace Gathering.Api.Endpoints.Sessions;

public sealed class GetActive : IEndpoint
{
  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapGet("/sessions/active", async (ISender sender, CancellationToken cancellationToken = default) =>
    {
      var query = new GetActiveSessionsQuery();

      var result = await sender.Send(query, cancellationToken);

      return result.Match(Results.Ok, CustomResults.Problem);
    })
    .WithTags(ApiTags.Session);
  }
}
