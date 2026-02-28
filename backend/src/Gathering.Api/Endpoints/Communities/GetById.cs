using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Communities.GetById;

namespace Gathering.Api.Endpoints.Communities;

public sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/communities/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var communityByIdQuery = new GetCommunityByIdQuery(id);

            var community = await sender.Send(communityByIdQuery, cancellationToken);

            return community.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(ApiTags.Community);
    }
}
