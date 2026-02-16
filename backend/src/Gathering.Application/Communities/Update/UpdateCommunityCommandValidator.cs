using FluentValidation;

namespace Gathering.Application.Communities.Update;

public sealed class UpdateCommunityCommandValidator : AbstractValidator<UpdateCommunityCommand>
{
    public UpdateCommunityCommandValidator()
    {
        RuleFor(x => x.CommunityId)
            .NotEmpty().WithMessage("Community ID cannot be empty.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Community name cannot be empty.")
            .MaximumLength(200).WithMessage("Community name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Community description cannot be empty.")
            .MaximumLength(1000).WithMessage("Community description cannot exceed 1000 characters.");

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