using Gathering.Api.Extensions;
using Gathering.Application.Abstractions;
using Gathering.Application.Sessions.Create;
using Gathering.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace Gathering.Api.Endpoints.Sessions;

public class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/sessions", Handler)
            .WithTags(ApiTags.Session)
            .Accepts<IFormCollection>("multipart/form-data")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateSession")
            .DisableAntiforgery(); // Disable CSRF protection for API endpoints
    }

    private static async Task<IResult> Handler(
        [FromForm] Guid communityId,
        [FromForm] string title,
        [FromForm] string description,
        [FromForm] string speaker,
        [FromForm] DateTime schedule,
        [FromForm(Name = "image")] IFormFile? image,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        Stream? imageStream = null;
        string? imageFileName = null;
        string? imageContentType = null;

        if (image is not null)
        {
            imageStream = image.OpenReadStream();
            imageFileName = image.FileName;
            imageContentType = image.ContentType;
        }

        var command = new CreateSessionCommand(
            communityId,
            title,
            description,
            speaker,
            schedule,
            imageStream,
            imageFileName,
            imageContentType);

        Result<Guid> result = await sender.Send(command, cancellationToken);

        if (imageStream is not null)
        {
            await imageStream.DisposeAsync();
        }

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}

