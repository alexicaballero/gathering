using Gathering.Api.Extensions;
using Gathering.Api.Infrastructure;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Resources.Update;
using Gathering.SharedKernel;

namespace Gathering.Api.Endpoints.Sessions.Resources;

public sealed class Update : IEndpoint
{
  public sealed record Request(string? Url, string? Notes, string? Title);

  public void MapEndpoint(IEndpointRouteBuilder app)
  {
    app.MapPut("/sessions/{sessionId:guid}/resources/{resourceId:guid}", async (
        Guid sessionId,
        Guid resourceId,
        Request request,
        ISender sender,
        CancellationToken cancellationToken = default) =>
    {
      var command = new UpdateSessionResourceCommand(
              sessionId,
              resourceId,
              request.Url,
              request.Notes,
              request.Title);

      Result<Guid> result = await sender.Send(command, cancellationToken);

      return result.Match(Results.Ok, CustomResults.Problem);
    })
    .WithTags(ApiTags.SessionResource);
  }
}
