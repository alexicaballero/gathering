using Gathering.Api.Extensions;
using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Delete;
using Gathering.SharedKernel;

namespace Gathering.Api.Endpoints.Communities;

public sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/communities/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var deleteCommand = new DeleteCommunityCommand(id);

            Result result = await sender.Send(deleteCommand, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).WithTags(ApiTags.Community);
    }
}
