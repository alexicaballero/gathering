using FluentValidation;
using Gathering.Application.Abstractions;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.Resources.Create;

public sealed class CreateSessionResourceCommandHandler : ICommandHandler<CreateSessionResourceCommand, Guid>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSessionResourceCommand> _validator;

    public CreateSessionResourceCommandHandler(
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateSessionResourceCommand> validator)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> HandleAsync(CreateSessionResourceCommand request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<Guid>(Error.Validation("CreateSessionResource.ValidationFailed", errorMessages));
        }

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            return Result.Failure<Guid>(SessionError.NotFound);
        }

        var addResult = session.AddResource(request.Type, request.Url, request.Notes, request.Title);
        if (addResult.IsFailure)
        {
            return Result.Failure<Guid>(addResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(addResult.Value.Id);
    }
}
