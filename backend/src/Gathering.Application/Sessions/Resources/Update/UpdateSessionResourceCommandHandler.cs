using FluentValidation;
using Gathering.Application.Abstractions;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.Resources.Update;

public sealed class UpdateSessionResourceCommandHandler : ICommandHandler<UpdateSessionResourceCommand, Guid>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateSessionResourceCommand> _validator;

    public UpdateSessionResourceCommandHandler(
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateSessionResourceCommand> validator)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> HandleAsync(UpdateSessionResourceCommand request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<Guid>(Error.Validation("UpdateSessionResource.ValidationFailed", errorMessages));
        }

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            return Result.Failure<Guid>(SessionError.NotFound);
        }

        var updateResult = session.UpdateResource(request.ResourceId, request.Url, request.Notes, request.Title);
        if (updateResult.IsFailure)
        {
            return Result.Failure<Guid>(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(request.ResourceId);
    }
}
