using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.GetByCommunity;

namespace Gathering.Api.Endpoints.Sessions;

public sealed class GetByCommunity : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions/community/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var sessionsByCommunityQuery = new GetSessionsByCommunityQuery(id);

            var sessionsResult = await sender.Send(sessionsByCommunityQuery, cancellationToken);

            return sessionsResult.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(ApiTags.Session);
    }
}
