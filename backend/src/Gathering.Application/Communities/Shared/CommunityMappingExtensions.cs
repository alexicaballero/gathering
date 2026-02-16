using Gathering.Domain.Communities;

namespace Gathering.Application.Communities.Shared;

internal static class CommunityMappingExtensions
{
    public static CommunityResponse ToResponse(this Community community)
    {
        return new CommunityResponse(
            community.Id,
            community.Name,
            community.Description,
            community.Image,
            community.Sessions.Select(s => s.ToSessionResponse()).ToList());
    }

    public static IReadOnlyList<CommunityResponse> ToResponse(this IEnumerable<Community> communities)
    {
        return communities.Select(c => c.ToResponse()).ToList();
    }

    private static SessionResponse ToSessionResponse(this Domain.Sessions.Session session)
    {
        return new SessionResponse(
            session.Id,
            session.Title,
            session.Description,
            session.Speaker,
            session.Schedule,
            session.State.ToString());
    }
}