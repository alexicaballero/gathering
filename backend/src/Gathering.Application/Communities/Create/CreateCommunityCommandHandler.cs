using FluentValidation;
using Gathering.Application.Abstractions;
using Gathering.Application.Common.Validators;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.SharedKernel;

namespace Gathering.Application.Communities.Create;

public sealed class CreateCommunityCommandHandler : ICommandHandler<CreateCommunityCommand, Guid>
{
    private readonly ICommunityRepository _communityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCommunityCommand> _validator;
    private readonly IImageStorageService _imageStorageService;

    public CreateCommunityCommandHandler(
        ICommunityRepository communityRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCommunityCommand> validator,
        IImageStorageService imageStorageService)
    {
        _communityRepository = communityRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<Guid>> HandleAsync(CreateCommunityCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<Guid>(Error.Validation("CreateCommunity.ValidationFailed", errorMessages));
        }

        // Handle image upload if provided
        string? imageUrl = null;
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

            // Upload image to Azure Blob Storage
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

            imageUrl = uploadResult.Value;
        }

        // Create the community with the optional image URL
        var result = Community.Create(command.Name, command.Description, imageUrl);

        if (result.IsFailure)
        {
            // If community creation failed and we uploaded an image, delete it
            if (imageUrl is not null)
            {
                await _imageStorageService.DeleteImageAsync(imageUrl, cancellationToken);
            }
            return Result.Failure<Guid>(result.Error);
        }

        _communityRepository.Add(result.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
