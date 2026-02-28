using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Domain.Communities;

public sealed class Community : AuditableEntity
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? Image { get; private set; }

    private readonly List<Session> _sessions = [];

    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

    private Community() { }

    public static Result<Community> Create(string name, string description, string? image = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Community>(CommunityError.NameEmpty);
        }

        if (name.Length > 200)
        {
            return Result.Failure<Community>(CommunityError.NameTooLong);
        }

        if (string.IsNullOrWhiteSpace(description))
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
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(CommunityError.NameEmpty);
        }

        if (name.Length > 200)
        {
            return Result.Failure(CommunityError.NameTooLong);
        }

        if (string.IsNullOrWhiteSpace(description))
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
    public Result AddSession(Session session)
    {
        if (session.CommunityId != Id)
        {
            return Result.Failure(CommunityError.SessionNotBelongsToCommunity);
        }

        if (_sessions.Any(s => s.Id == session.Id))
        {
            return Result.Failure(CommunityError.SessionAlreadyExists);
        }

        _sessions.Add(session);

        return Result.Success();
    }

    /// <summary>
    /// Removes a session from the community.
    /// </summary>
    public Result RemoveSession(Guid sessionId)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);

        if (session is null)
        {
            return Result.Failure(CommunityError.SessionNotFound);
        }

        _sessions.Remove(session);

        return Result.Success();
    }
}
