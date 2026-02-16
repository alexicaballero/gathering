namespace Gathering.Application.Abstractions;

public interface ISender
{
    /// <summary>
    /// Sends the specified request and returns a response asynchronously.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends the specified request without expecting a response asynchronously.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
}