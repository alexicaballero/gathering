using Gathering.Application.Abstractions;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;

namespace Gathering.Application.Communities.Delete;

public sealed class DeleteCommunityCommandHandler : ICommandHandler<DeleteCommunityCommand>
{
    private readonly ICommunityRepository _communityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorageService;

    public DeleteCommunityCommandHandler(
        ICommunityRepository communityRepository,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorageService)
    {
        _communityRepository = communityRepository;
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result> HandleAsync(DeleteCommunityCommand request, CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(request.CommunityId);

        if (community == null)
        {
            return Result.Failure(CommunityError.NotFound);
        }

        // Delete image if it exists and is a blob URL (not an external URL like Unsplash)
        if (!string.IsNullOrEmpty(community.Image) && community.Image.Contains("blob.core.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            await _imageStorageService.DeleteImageAsync(community.Image, cancellationToken);
        }

        _communityRepository.Remove(community);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}