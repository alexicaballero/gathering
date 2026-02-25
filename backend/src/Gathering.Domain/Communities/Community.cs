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

    /// <summary>
    /// Adds a session to the community.
    /// Validates that the session belongs to this community and is not already in the list.
    /// </summary>
    /// <param name="session">The session to add</param>
    /// <returns>A result indicating success or failure</returns>
    public Result AddSession(Session session)
    {
        // Validate that the session belongs to this community
        if (session.CommunityId != Id)
        {
            return Result.Failure(CommunityError.SessionNotBelongsToCommunity);
        }

        // Validate that the session is not already in the list
        if (Sessions.Any(s => s.Id == session.Id))
        {
            return Result.Failure(CommunityError.SessionAlreadyExists);
        }

        Sessions.Add(session);

        return Result.Success();
    }

    /// <summary>
    /// Removes a session from the community.
    /// </summary>
    /// <param name="sessionId">The ID of the session to remove</param>
    /// <returns>A result indicating success or failure</returns>
    public Result RemoveSession(Guid sessionId)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId);

        if (session is null)
        {
            return Result.Failure(CommunityError.SessionNotFound);
        }

        Sessions.Remove(session);

        return Result.Success();
    }
}
