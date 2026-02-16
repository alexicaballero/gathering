using Gathering.Application.Abstractions;

namespace Gathering.Application.Sessions.GetById;

public sealed record GetSessionByIdQuery(Guid Id) : IQuery<GetSessionByIdQueryResponse>;