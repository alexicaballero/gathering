using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.GetById;

namespace Gathering.Api.Endpoints.Sessions;

public class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var sessionQuery = new GetSessionByIdQuery(id);

            var sessionResult = await sender.Send(sessionQuery, cancellationToken);

            if (sessionResult.IsFailure)
            {
                return Results.NotFound(sessionResult.Error);
            }

            return Results.Ok(sessionResult.Value);
        })
        .WithTags(ApiTags.Session);
    }
}
