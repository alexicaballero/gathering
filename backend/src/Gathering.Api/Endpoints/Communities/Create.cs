using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Create;
using Microsoft.AspNetCore.Mvc;

namespace Gathering.Api.Endpoints.Communities;

public class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/communities", Handler)
            .WithTags(ApiTags.Community)
            .Accepts<IFormCollection>("multipart/form-data")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateCommunity");
    }

    private static async Task<IResult> Handler(
        [FromForm] string name,
        [FromForm] string description,
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

        var command = new CreateCommunityCommand(name, description, imageStream, imageFileName, imageContentType);

        var result = await sender.Send(command, cancellationToken);

        if (imageStream is not null)
        {
            await imageStream.DisposeAsync();
        }

        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(new { error = result.Error.Description });
    }
}
