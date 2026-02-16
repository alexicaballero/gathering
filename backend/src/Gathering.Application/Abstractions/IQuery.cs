using Gathering.SharedKernel;

namespace Gathering.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}