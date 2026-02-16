using Gathering.Api.Endpoints;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Delete;


public sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/sessions/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var deleteCommand = new DeleteSessionCommand(id);

            await sender.Send(deleteCommand, cancellationToken);

            return Results.Ok();

        }).WithTags(ApiTags.Session);
    }
}
