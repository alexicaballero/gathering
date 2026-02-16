using Gathering.Application.Abstractions;

namespace Gathering.Application.Sessions.Delete;

public sealed record DeleteSessionCommand(Guid SessionId) : ICommand;
