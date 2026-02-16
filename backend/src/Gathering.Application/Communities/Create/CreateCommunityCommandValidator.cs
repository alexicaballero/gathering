using FluentValidation;

namespace Gathering.Application.Communities.Create;

public sealed class CreateCommunityCommandValidator : AbstractValidator<CreateCommunityCommand>
{
    public CreateCommunityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Community name is required.")
            .MaximumLength(100)
            .WithMessage("Community name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-_.]+$")
            .WithMessage("Community name contains invalid characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Community description is required.")
            .MaximumLength(500)
            .WithMessage("Community description cannot exceed 500 characters.")
            .MinimumLength(10)
            .WithMessage("Community description must be at least 10 characters.");

        // Image validation at fluent level: if image stream is provided, fileName and contentType must also be provided
        When(x => x.ImageStream is not null, () =>
        {
            RuleFor(x => x.ImageFileName)
                .NotEmpty()
                .WithMessage("Image file name is required when uploading an image.");

            RuleFor(x => x.ImageContentType)
                .NotEmpty()
                .WithMessage("Image content type is required when uploading an image.");
        });
    }
}

