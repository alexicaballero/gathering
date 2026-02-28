namespace Gathering.SharedKernel;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
