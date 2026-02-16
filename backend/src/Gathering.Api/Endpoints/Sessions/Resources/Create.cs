using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Resources.Create;
using Gathering.Domain.Sessions;

namespace Gathering.Api.Endpoints.Sessions.Resources;

public sealed class Create : IEndpoint
{
  public sealed record Request(SessionResourceType Type, string? Url, string? Notes, string? Title);

  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapPost("/sessions/{sessionId:guid}/resources", async (Guid sessionId, Request request, ISender sender, CancellationToken cancellationToken = default) =>
    {
      var command = new CreateSessionResourceCommand(
              sessionId,
              request.Type,
              request.Url,
              request.Notes,
              request.Title);

      var result = await sender.Send(command, cancellationToken);

      if (result.IsFailure)
      {
        return Results.BadRequest(result.Error);
      }

      return Results.Ok(new { Id = result.Value });
    })
    .WithTags(ApiTags.SessionResource);
  }
}
