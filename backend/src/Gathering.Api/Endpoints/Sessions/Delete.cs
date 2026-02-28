using Gathering.Api.Endpoints;
using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Delete;
using Gathering.SharedKernel;

namespace Gathering.Api.Endpoints.Sessions;

public sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/sessions/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var deleteCommand = new DeleteSessionCommand(id);

            Result result = await sender.Send(deleteCommand, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).WithTags(ApiTags.Session);
    }
}
