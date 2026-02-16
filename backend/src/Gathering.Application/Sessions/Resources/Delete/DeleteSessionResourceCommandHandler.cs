using Gathering.Application.Abstractions;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.Resources.Delete;

public sealed class DeleteSessionResourceCommandHandler : ICommandHandler<DeleteSessionResourceCommand>
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUnitOfWork _unitOfWork;

  public DeleteSessionResourceCommandHandler(
      ISessionRepository sessionRepository,
      IUnitOfWork unitOfWork)
  {
    _sessionRepository = sessionRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<Result> HandleAsync(DeleteSessionResourceCommand request, CancellationToken cancellationToken = default)
  {
    var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
    if (session is null)
    {
      return Result.Failure(SessionError.NotFound);
    }

    var removeResult = session.RemoveResource(request.ResourceId);
    if (removeResult.IsFailure)
    {
      return Result.Failure(removeResult.Error);
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}
