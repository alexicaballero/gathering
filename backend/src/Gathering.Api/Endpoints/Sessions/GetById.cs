using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.GetById;

namespace Gathering.Api.Endpoints.Sessions;

public sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/sessions/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken = default) =>
        {
            var sessionQuery = new GetSessionByIdQuery(id);

            var sessionResult = await sender.Send(sessionQuery, cancellationToken);

            return sessionResult.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(ApiTags.Session);
    }
}
