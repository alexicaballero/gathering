using FluentValidation;

namespace Gathering.Application.Sessions.Create;

public sealed class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.CommunityId)
            .NotEmpty()
            .WithMessage("Community ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Session title is required.")
            .MaximumLength(200)
            .WithMessage("Session title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Session description cannot exceed 1000 characters.");

        RuleFor(x => x.Speaker)
            .NotEmpty()
            .WithMessage("Session speaker is required.")
            .MaximumLength(100)
            .WithMessage("Speaker name cannot exceed 100 characters.");

        RuleFor(x => x.ScheduledAt)
            .NotEmpty()
            .WithMessage("Session schedule is required.")
            .Must(scheduledAt => scheduledAt > DateTimeOffset.UtcNow)
            .WithMessage("Session schedule must be in the future.");

        // Image validation: if image stream is provided, fileName and contentType must also be provided
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