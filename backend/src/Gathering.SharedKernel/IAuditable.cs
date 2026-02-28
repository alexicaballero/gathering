namespace Gathering.SharedKernel;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }

    DateTimeOffset? UpdatedAt { get; }
}
