namespace Gathering.Application.Communities.Shared;

public sealed record CommunityResponse(
  Guid Id,
  string Name,
  string Description,
  string? Image,
  IReadOnlyList<SessionResponse> Sessions);

public sealed record SessionResponse(
  Guid Id,
  string Title,
  string Description,
  string Speaker,
  DateTime Schedule,
  string State);