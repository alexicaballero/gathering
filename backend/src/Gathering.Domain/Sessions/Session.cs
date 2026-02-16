

using Gathering.SharedKernel;

namespace Gathering.Domain.Sessions;

public partial class Session : AuditableEntity
{
    public Guid Id { get; private set; }

    public Guid CommunityId { get; private set; } = Guid.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? Image { get; private set; }

    public string Speaker { get; private set; } = string.Empty;

    public DateTime Schedule { get; private set; }

    public SessionState State { get; private set; }

    private Session() { }

    public static Result<Session> Create(Guid CommunityId, string title, string description, string speaker, DateTime schedule, string? Image)
    {
        // Community validations

        if (CommunityId == Guid.Empty)
        {
            return Result.Failure<Session>(SessionError.ComunityInvalid);
        }

        // Title validations

        if (string.IsNullOrWhiteSpace(title.Trim()))
        {
            return Result.Failure<Session>(SessionError.TitleEmpty);
        }

        if (title.Length > 200)
        {
            return Result.Failure<Session>(SessionError.TitleTooLong);
        }

        // Description validations

        if (string.IsNullOrWhiteSpace(description.Trim()))
        {
            return Result.Failure<Session>(SessionError.DescriptionEmpty);
        }

        if (description.Length > 1000)
        {
            return Result.Failure<Session>(SessionError.DescriptionTooLong);
        }

        // Speaker validations

        if (string.IsNullOrWhiteSpace(speaker.Trim()))
        {
            return Result.Failure<Session>(SessionError.SpeakerEmpty);
        }

        // Schedule validations
        if (schedule <= DateTime.UtcNow)
        {
            return Result.Failure<Session>(SessionError.ScheduleInvalid);
        }

        var session = new Session
        {
            Id = Guid.NewGuid(),
            CommunityId = CommunityId,
            Title = title,
            Description = description,
            Speaker = speaker,
            Schedule = schedule,
            Image = Image,
            State = SessionState.Scheduled
        };

        session.Raise(new SessionCreatedDomainEvent(session.Id));

        return Result.Success(session);
    }

    public Result Update(string title, string description, string speaker, DateTime schedule, string? image = null)
    {
        // Title validations
        if (string.IsNullOrWhiteSpace(title.Trim()))
        {
            return Result.Failure(SessionError.TitleEmpty);
        }

        if (title.Length > 200)
        {
            return Result.Failure(SessionError.TitleTooLong);
        }

        // Description validations
        if (string.IsNullOrWhiteSpace(description.Trim()))
        {
            return Result.Failure(SessionError.DescriptionEmpty);
        }

        if (description.Length > 1000)
        {
            return Result.Failure(SessionError.DescriptionTooLong);
        }

        // Speaker validations
        if (string.IsNullOrWhiteSpace(speaker.Trim()))
        {
            return Result.Failure(SessionError.SpeakerEmpty);
        }

        // Schedule validations - for updates, allow past dates (completed sessions)
        // Only prevent scheduling in the past for new sessions

        Title = title;
        Description = description;
        Speaker = speaker;
        Schedule = schedule;
        Image = image;

        return Result.Success();
    }

    public void UpdateState(SessionState state)
    {
        State = state;
    }
}
