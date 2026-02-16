using Gathering.Application.Abstractions;

namespace Gathering.Application.Sessions.Resources.Delete;

public sealed record DeleteSessionResourceCommand(Guid SessionId, Guid ResourceId) : ICommand;
