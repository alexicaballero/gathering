namespace Gathering.Application.Abstractions;

/// <summary>
/// Marker interface to represent a request with a void response.
/// Used for commands or queries that do not return a value.
/// </summary>
public interface IRequest : IBaseRequest { }

/// <summary>
/// Marker interface to represent a request with a response.
/// Used for commands or queries that return a value of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the request.</typeparam>
public interface IRequest<out TResponse> : IBaseRequest { }

/// <summary>
/// Base marker interface for all request types.
/// Enables generic constraints for objects implementing <see cref="IRequest"/> or <see cref="IRequest{TResponse}"/>.
/// </summary>
public interface IBaseRequest { }
