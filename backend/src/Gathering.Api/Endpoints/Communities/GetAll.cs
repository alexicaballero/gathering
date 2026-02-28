using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Communities.GetAll;

namespace Gathering.Api.Endpoints.Communities;

public sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/communities", async (ISender sender, CancellationToken cancellationToken = default) =>
        {
            var communitiesQuery = new GetCommunitiesQuery();

            var communities = await sender.Send(communitiesQuery, cancellationToken);

            return communities.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(ApiTags.Community);
    }
}
