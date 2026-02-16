using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Delete;

namespace Gathering.Api.Endpoints.Communities;

public sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/communities/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var deleteCommand = new DeleteCommunityCommand(id);

            await sender.Send(deleteCommand, cancellationToken);

            return Results.Ok();

        }).WithTags(ApiTags.Community);
    }
}
