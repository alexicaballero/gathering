using Gathering.Application.Abstractions;
using Gathering.Domain.Sessions;

namespace Gathering.Application.Sessions.Resources.Create;

public sealed record CreateSessionResourceCommand(
    Guid SessionId,
    SessionResourceType Type,
    string? Url,
    string? Notes,
    string? Title = null) : ICommand<Guid>;
