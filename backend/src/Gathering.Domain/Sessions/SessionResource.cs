using Gathering.SharedKernel;

namespace Gathering.Domain.Sessions;

public sealed class SessionResource : AuditableEntity
{
  public Guid Id { get; private set; }

  public Guid SessionId { get; private set; }

  public SessionResourceType Type { get; private set; }

  public string? Title { get; private set; }

  public string? Url { get; private set; }

  public string? Notes { get; private set; }

  private SessionResource() { }

  internal static Result<SessionResource> Create(
      Guid sessionId,
      SessionResourceType type,
      string? url,
        string? notes,
        string? title)
  {
    var validation = Validate(type, url, notes, title);
    if (validation.IsFailure)
    {
      return Result.Failure<SessionResource>(validation.Error);
    }

    var resource = new SessionResource
    {
      Id = Guid.NewGuid(),
      SessionId = sessionId,
      Type = type,
      Title = title,
      Url = RequiresUrl(type) ? url : null,
      Notes = RequiresNotes(type) ? notes : null
    };

    return Result.Success(resource);
  }

  internal Result Update(string? url, string? notes, string? title)
  {
    var validation = Validate(Type, url, notes, title);
    if (validation.IsFailure)
    {
      return validation;
    }

    Title = title;
    Url = RequiresUrl(Type) ? url : null;
    Notes = RequiresNotes(Type) ? notes : null;

    return Result.Success();
  }

  private static Result Validate(SessionResourceType type, string? url, string? notes, string? title)
  {
    if (!Enum.IsDefined(type))
    {
      return Result.Failure(SessionError.ResourceTypeInvalid);
    }

    if (!string.IsNullOrWhiteSpace(title) && title.Length > 200)
    {
      return Result.Failure(SessionError.ResourceTitleTooLong);
    }

    if (RequiresUrl(type))
    {
      if (string.IsNullOrWhiteSpace(url))
      {
        return Result.Failure(SessionError.ResourceUrlEmpty);
      }

      if (url.Length > 2000)
      {
        return Result.Failure(SessionError.ResourceUrlTooLong);
      }
    }

    if (RequiresNotes(type))
    {
      if (string.IsNullOrWhiteSpace(notes))
      {
        return Result.Failure(SessionError.ResourceNotesEmpty);
      }

      if (notes.Length > 4000)
      {
        return Result.Failure(SessionError.ResourceNotesTooLong);
      }
    }

    return Result.Success();
  }

  private static bool RequiresUrl(SessionResourceType type) =>
      type is SessionResourceType.Video or SessionResourceType.ExternalLink;

  private static bool RequiresNotes(SessionResourceType type) =>
      type is SessionResourceType.Notes;
}
