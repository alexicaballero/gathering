using Gathering.Application.Abstractions;
using Gathering.Application.Communities.Shared;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;

namespace Gathering.Application.Communities.GetAll;

public sealed class GetCommunitiesQueryHandler : IQueryHandler<GetCommunitiesQuery, IReadOnlyList<CommunityResponse>>
{
    private readonly ICommunityRepository _communityRepository;

    public GetCommunitiesQueryHandler(ICommunityRepository communityRepository)
    {
        _communityRepository = communityRepository;
    }

    public async Task<Result<IReadOnlyList<CommunityResponse>>> HandleAsync(GetCommunitiesQuery request, CancellationToken cancellationToken = default)
    {
        var communities = await _communityRepository.GetAllAsync(cancellationToken);

        var response = communities.ToResponse();

        return Result.Success(response);
    }
}