using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Domain.Communities;

public class Community : AuditableEntity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? Image { get; private set; }

    public List<Session> Sessions { get; private set; } = [];

    private Community() { }

    public static Result<Community> Create(string name, string description, string? image = null)
    {
        // Validation for Name
        if (string.IsNullOrWhiteSpace(name.Trim()))
        {
            return Result.Failure<Community>(CommunityError.NameEmpty);
        }

        if (name.Length > 200)
        {
            return Result.Failure<Community>(CommunityError.NameTooLong);
        }

        // Validation for Description
        if (string.IsNullOrWhiteSpace(description.Trim()))
        {
            return Result.Failure<Community>(CommunityError.DescriptionEmpty);
        }

        if (description.Length > 1000)
        {
            return Result.Failure<Community>(CommunityError.DescriptionTooLong);
        }

        var community = new Community
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Image = image
        };

        community.Raise(new CommunityCreatedDomainEvent(community.Id));

        return Result.Success(community);
    }

    public Result Update(string name, string description, string? image = null)
    {
        // Validation for Name
        if (string.IsNullOrWhiteSpace(name.Trim()))
        {
            return Result.Failure(CommunityError.NameEmpty);
        }

        if (name.Length > 200)
        {
            return Result.Failure(CommunityError.NameTooLong);
        }

        // Validation for Description
        if (string.IsNullOrWhiteSpace(description.Trim()))
        {
            return Result.Failure(CommunityError.DescriptionEmpty);
        }

        if (description.Length > 1000)
        {
            return Result.Failure(CommunityError.DescriptionTooLong);
        }

        Name = name;
        Description = description;
        Image = image;

        return Result.Success();
    }
}
