using Gathering.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Gathering.Application.Mediator;

public sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        return await handler.HandleAsync((dynamic)request, cancellationToken);
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(void));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        await handler.HandleAsync((dynamic)request, cancellationToken);
    }
}
