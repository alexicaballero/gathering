

using Gathering.SharedKernel;

namespace Gathering.Domain.Sessions;

public static class SessionError
{
    public static readonly Error CommunityInvalid = Error.Validation("Session.Community.Invalid", "Session must belong to a valid community");

    public static readonly Error TitleEmpty = Error.Validation("Session.Title.Empty", "Session title cannot be empty");

    public static readonly Error TitleTooLong = Error.Validation("Session.Title.TooLong", "Session title cannot exceed 200 characters");

    public static readonly Error DescriptionTooLong = Error.Validation("Session.Description.TooLong", "Session description cannot exceed 1000 characters");

    public static readonly Error SpeakerEmpty = Error.Validation("Session.Speaker.Empty", "Session speaker cannot be empty");

    public static readonly Error ScheduleInvalid = Error.Validation("Session.Schedule.Invalid", "Session schedule must be a future date and time");

    public static readonly Error NotFound = Error.NotFound("Session.NotFound", "The specified session was not found");

    public static readonly Error InvalidStatusTransition = Error.Validation("Session.Status.InvalidTransition", "The requested status transition is not allowed");

    public static readonly Error ResourceNotFound = Error.NotFound("Session.Resource.NotFound", "The specified session resource was not found");

    public static readonly Error ResourceTypeInvalid = Error.Validation("Session.Resource.Type.Invalid", "Session resource type is invalid");

    public static readonly Error ResourceTitleTooLong = Error.Validation("Session.Resource.Title.TooLong", "Session resource title cannot exceed 200 characters");

    public static readonly Error ResourceUrlEmpty = Error.Validation("Session.Resource.Url.Empty", "Session resource URL cannot be empty");

    public static readonly Error ResourceUrlTooLong = Error.Validation("Session.Resource.Url.TooLong", "Session resource URL cannot exceed 2000 characters");

    public static readonly Error ResourceNotesEmpty = Error.Validation("Session.Resource.Notes.Empty", "Session resource notes cannot be empty");

    public static readonly Error ResourceNotesTooLong = Error.Validation("Session.Resource.Notes.TooLong", "Session resource notes cannot exceed 4000 characters");

    public static readonly Error ImageUploadFailed = Error.Problem("Session.Image.UploadFailed", "Failed to upload session image to storage");

    public static readonly Error ImageDeleteFailed = Error.Problem("Session.Image.DeleteFailed", "Failed to delete session image from storage");
}
