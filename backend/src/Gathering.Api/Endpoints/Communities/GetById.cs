using Gathering.Application.Abstractions;
using Gathering.Application.Communities.GetById;

namespace Gathering.Api.Endpoints.Communities;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/Communities/{id:guid}", async (Guid Id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var communityByIdQuery = new GetCommunityByIdQuery(Id);

            var community = await sender.Send(communityByIdQuery, cancellationToken);

            return Results.Ok(community.Value);
        })
        .WithTags(ApiTags.Community);
    }
}
