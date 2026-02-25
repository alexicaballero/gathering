using FluentValidation;
using Gathering.Application.Abstractions;
using Gathering.Application.Common.Validators;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;

namespace Gathering.Application.Communities.Update;

public sealed class UpdateCommunityCommandHandler : ICommandHandler<UpdateCommunityCommand, Guid>
{
    private readonly ICommunityRepository _communityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateCommunityCommand> _validator;
    private readonly IImageStorageService _imageStorageService;

    public UpdateCommunityCommandHandler(
        ICommunityRepository communityRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateCommunityCommand> validator,
        IImageStorageService imageStorageService)
    {
        _communityRepository = communityRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<Guid>> HandleAsync(UpdateCommunityCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<Guid>(Error.Validation("UpdateCommunity.ValidationFailed", errorMessages));
        }

        var community = await _communityRepository.GetByIdAsync(command.CommunityId);

        if (community is null)
        {
            return Result.Failure<Guid>(CommunityError.NotFound);
        }

        // Handle image upload if a new image is provided
        string? newImageUrl = community.Image; // Keep existing image by default
        if (command.ImageStream is not null && command.ImageFileName is not null && command.ImageContentType is not null)
        {
            // Validate image file
            var fileValidationResult = FileValidator.ValidateImageFile(
                command.ImageStream,
                command.ImageFileName,
                command.ImageContentType);

            if (fileValidationResult.IsFailure)
            {
                return Result.Failure<Guid>(fileValidationResult.Error);
            }

            // Upload new image to Azure Blob Storage
            var uploadResult = await _imageStorageService.UploadImageAsync(
                command.ImageStream,
                command.ImageFileName,
                command.ImageContentType,
                "communities",
                cancellationToken);

            if (uploadResult.IsFailure)
            {
                return Result.Failure<Guid>(uploadResult.Error);
            }

            newImageUrl = uploadResult.Value;

            // Delete old image if it exists and is a blob URL (not an external URL like Unsplash)
            if (!string.IsNullOrEmpty(community.Image) && community.Image.Contains("blob.core.windows.net", StringComparison.OrdinalIgnoreCase))
            {
                await _imageStorageService.DeleteImageAsync(community.Image, cancellationToken);
            }
        }

        var updateResult = community.Update(
            command.Name,
            command.Description,
            newImageUrl);

        if (updateResult.IsFailure)
        {
            return Result.Failure<Guid>(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(community.Id);
    }
}
