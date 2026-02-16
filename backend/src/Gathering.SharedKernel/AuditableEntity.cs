namespace Gathering.SharedKernel;

public abstract class AuditableEntity : Entity, IAuditable
{
    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}
