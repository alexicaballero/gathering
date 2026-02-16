using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Shared;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;

namespace Gathering.Application.Communities.GetById;

public sealed class GetCommunityByIdQueryHandler : IQueryHandler<GetCommunityByIdQuery, CommunityResponse>
{
    private readonly ICommunityRepository _communityRepository;

    public GetCommunityByIdQueryHandler(ICommunityRepository communityRepository)
    {
        _communityRepository = communityRepository;
    }

    public async Task<Result<CommunityResponse>> HandleAsync(GetCommunityByIdQuery request, CancellationToken cancellationToken = default)
    {
        var community = await _communityRepository.GetByIdAsync(request.CommunityId);

        if (community is null)
        {
            return Result.Failure<CommunityResponse>(Domain.Communities.CommunityError.NotFound);
        }

        var result = community.ToResponse();

        return Result.Success(result);
    }
}