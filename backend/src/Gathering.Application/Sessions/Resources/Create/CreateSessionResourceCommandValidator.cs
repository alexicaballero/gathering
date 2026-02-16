using FluentValidation;
using Gathering.Domain.Sessions;

namespace Gathering.Application.Sessions.Resources.Create;

public sealed class CreateSessionResourceCommandValidator : AbstractValidator<CreateSessionResourceCommand>
{
  public CreateSessionResourceCommandValidator()
  {
    RuleFor(x => x.SessionId)
        .NotEmpty().WithMessage("Session ID is required.");

    RuleFor(x => x.Type)
        .IsInEnum().WithMessage("Session resource type is invalid.");

    RuleFor(x => x.Title)
        .MaximumLength(200).WithMessage("Session resource title cannot exceed 200 characters.")
        .When(x => !string.IsNullOrWhiteSpace(x.Title));

    RuleFor(x => x.Url)
        .MaximumLength(2000).WithMessage("Session resource URL cannot exceed 2000 characters.")
        .When(x => !string.IsNullOrWhiteSpace(x.Url));

    RuleFor(x => x.Notes)
        .MaximumLength(4000).WithMessage("Session resource notes cannot exceed 4000 characters.")
        .When(x => !string.IsNullOrWhiteSpace(x.Notes));
  }
}
