using Gathering.SharedKernel;

namespace Gathering.Domain.Sessions;

public sealed partial class Session : AuditableEntity
{
    public Guid Id { get; private set; }

    public Guid CommunityId { get; private set; } = Guid.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? Image { get; private set; }

    public string Speaker { get; private set; } = string.Empty;

    public DateTimeOffset ScheduledAt { get; private set; }

    public SessionStatus Status { get; private set; }

    private Session() { }

    public static Result<Session> Create(
        Guid communityId,
        string title,
        string speaker,
        DateTimeOffset scheduledAt,
        string? description = null,
        string? image = null)
    {
        if (communityId == Guid.Empty)
        {
            return Result.Failure<Session>(SessionError.CommunityInvalid);
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Session>(SessionError.TitleEmpty);
        }

        if (title.Length > 200)
        {
            return Result.Failure<Session>(SessionError.TitleTooLong);
        }

        if (description is not null && description.Length > 1000)
        {
            return Result.Failure<Session>(SessionError.DescriptionTooLong);
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            return Result.Failure<Session>(SessionError.SpeakerEmpty);
        }

        if (scheduledAt <= DateTimeOffset.UtcNow)
        {
            return Result.Failure<Session>(SessionError.ScheduleInvalid);
        }

        var session = new Session
        {
            Id = Guid.NewGuid(),
            CommunityId = communityId,
            Title = title,
            Description = description,
            Speaker = speaker,
            ScheduledAt = scheduledAt,
            Image = image,
            Status = SessionStatus.Scheduled
        };

        session.Raise(new SessionCreatedDomainEvent(session.Id));

        return Result.Success(session);
    }

    /// <summary>
    /// Creates a session without schedule validation. For seeding/testing only.
    /// </summary>
    internal static Result<Session> CreateCompleted(
        Guid communityId,
        string title,
        string speaker,
        DateTimeOffset scheduledAt,
        string? description = null,
        string? image = null)
    {
        if (communityId == Guid.Empty)
        {
            return Result.Failure<Session>(SessionError.CommunityInvalid);
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Session>(SessionError.TitleEmpty);
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            return Result.Failure<Session>(SessionError.SpeakerEmpty);
        }

        var session = new Session
        {
            Id = Guid.NewGuid(),
            CommunityId = communityId,
            Title = title,
            Description = description,
            Speaker = speaker,
            ScheduledAt = scheduledAt,
            Image = image,
            Status = SessionStatus.Completed
        };

        return Result.Success(session);
    }

    public Result Update(
        string title,
        string speaker,
        DateTimeOffset scheduledAt,
        string? description = null,
        string? image = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(SessionError.TitleEmpty);
        }

        if (title.Length > 200)
        {
            return Result.Failure(SessionError.TitleTooLong);
        }

        if (description is not null && description.Length > 1000)
        {
            return Result.Failure(SessionError.DescriptionTooLong);
        }

        if (string.IsNullOrWhiteSpace(speaker))
        {
            return Result.Failure(SessionError.SpeakerEmpty);
        }

        Title = title;
        Description = description;
        Speaker = speaker;
        ScheduledAt = scheduledAt;
        Image = image;

        return Result.Success();
    }

    public Result UpdateStatus(SessionStatus newStatus)
    {
        if (Status == SessionStatus.Canceled && newStatus == SessionStatus.Scheduled)
        {
            return Result.Failure(SessionError.InvalidStatusTransition);
        }

        if (Status == SessionStatus.Completed && newStatus == SessionStatus.Scheduled)
        {
            return Result.Failure(SessionError.InvalidStatusTransition);
        }

        Status = newStatus;
        return Result.Success();
    }
}
