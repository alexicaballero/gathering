using Gathering.Application.Abstractions;

namespace Gathering.Application.Communities.Delete;

public sealed record DeleteCommunityCommand(Guid CommunityId) : ICommand;
