using Gathering.SharedKernel;

namespace Gathering.Domain.Communities;

public static class CommunityError
{
    public static readonly Error NameEmpty = Error.Validation("Community.Name.Empty", "Community name cannot be empty");

    public static readonly Error NameTooLong = Error.Validation("Community.Name.TooLong", "Community name cannot exceed 200 characters");

    public static readonly Error DescriptionTooLong = Error.Validation("Community.Description.TooLong", "Community description cannot exceed 1000 characters");

    public static readonly Error DescriptionEmpty = Error.Validation("Community.Description.Empty", "Community description cannot be empty");

    public static readonly Error NotFound = Error.NotFound("Community.NotFound", "The specified community was not found");

    public static readonly Error ImageUploadFailed = Error.Problem("Community.Image.UploadFailed", "Failed to upload community image to storage");

    public static readonly Error ImageDeleteFailed = Error.Problem("Community.Image.DeleteFailed", "Failed to delete community image from storage");
}
