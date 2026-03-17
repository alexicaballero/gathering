using FluentAssertions;
using Gathering.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Gathering.Api.Tests.Middleware;

public class GlobalExceptionHandlerTests
{
  [Fact]
  public async Task TryHandleAsync_WhenExceptionOccurs_ShouldSetStatusCode500AndReturnTrue()
  {
    // Arrange
    var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
    var httpContext = new DefaultHttpContext();
    httpContext.Response.Body = new MemoryStream();
    var exception = new InvalidOperationException("Something went wrong");

    // Act
    var result = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

    // Assert
    result.Should().BeTrue();
    httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
  }

  [Fact]
  public async Task TryHandleAsync_ShouldReturnTrueForAnyException()
  {
    // Arrange
    var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
    var httpContext = new DefaultHttpContext();
    httpContext.Response.Body = new MemoryStream();

    // Act
    var result = await handler.TryHandleAsync(httpContext, new Exception("error"), CancellationToken.None);

    // Assert
    result.Should().BeTrue();
  }
}
