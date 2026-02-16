using Gathering.SharedKernel;

namespace Gathering.Domain.Sessions;

public partial class Session
{
  private readonly List<SessionResource> _resources = [];

  public IReadOnlyCollection<SessionResource> Resources => _resources.AsReadOnly();

  public Result<SessionResource> AddResource(
      SessionResourceType type,
      string? url,
      string? notes,
      string? title = null)
  {
    var resourceResult = SessionResource.Create(Id, type, url, notes, title);
    if (resourceResult.IsFailure)
    {
      return Result.Failure<SessionResource>(resourceResult.Error);
    }

    _resources.Add(resourceResult.Value);

    return Result.Success(resourceResult.Value);
  }

  public Result UpdateResource(Guid resourceId, string? url, string? notes, string? title = null)
  {
    var resource = _resources.FirstOrDefault(r => r.Id == resourceId);
    if (resource is null)
    {
      return Result.Failure(SessionError.ResourceNotFound);
    }

    return resource.Update(url, notes, title);
  }

  public Result RemoveResource(Guid resourceId)
  {
    var resource = _resources.FirstOrDefault(r => r.Id == resourceId);
    if (resource is null)
    {
      return Result.Failure(SessionError.ResourceNotFound);
    }

    _resources.Remove(resource);

    return Result.Success();
  }
}
