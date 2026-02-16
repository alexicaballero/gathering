using Gathering.Application.Abstractions;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.Delete;

public sealed class DeleteSessionCommandHandler : ICommandHandler<DeleteSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorageService;

    public DeleteSessionCommandHandler(
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorageService)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result> HandleAsync(DeleteSessionCommand request, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId);

        if (session == null)
        {
            return Result.Failure(SessionError.NotFound);
        }

        // Delete image if it exists and is a blob URL (not an external URL)
        if (!string.IsNullOrEmpty(session.Image) && session.Image.Contains("blob.core.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            await _imageStorageService.DeleteImageAsync(session.Image, cancellationToken);
        }

        _sessionRepository.Remove(session);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}