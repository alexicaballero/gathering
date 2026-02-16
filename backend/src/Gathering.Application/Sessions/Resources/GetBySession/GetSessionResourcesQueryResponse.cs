using Gathering.Domain.Sessions;

namespace Gathering.Application.Sessions.Resources.GetBySession;

public sealed record GetSessionResourcesQueryResponse(
    Guid Id,
    Guid SessionId,
    SessionResourceType Type,
    string? Title,
    string? Url,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
