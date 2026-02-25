using Gathering.Application.Abstractions;

namespace Gathering.Application.Sessions.Resources.Update;

public sealed record UpdateSessionResourceCommand(
    Guid SessionId,
    Guid ResourceId,
    string? Url,
    string? Notes,
    string? Title = null) : ICommand<Guid>;
