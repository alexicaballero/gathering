namespace Gathering.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
