using FluentValidation;
using Gathering.Domain.Sessions;

namespace Gathering.Application.Sessions.Update;

public sealed class UpdateSessionCommandValidator : AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID cannot be empty.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Session title cannot be empty.")
            .MaximumLength(200).WithMessage("Session title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Session description cannot exceed 1000 characters.");

        RuleFor(x => x.Speaker)
            .NotEmpty().WithMessage("Session speaker cannot be empty.");

        RuleFor(x => x.Status)
            .Must(BeValidStatus).WithMessage("Invalid session status.");

        RuleFor(x => x.ScheduledAt)
            .NotEmpty().WithMessage("Session schedule cannot be empty.");

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

    private static bool BeValidStatus(SessionStatus status)
    {
        return Enum.IsDefined(typeof(SessionStatus), status);
    }
}