namespace Gathering.SharedKernel;

public interface IAuditable
{
    DateTime CreatedAt { get; }

    DateTime? UpdatedAt { get; }
}
