using FluentValidation;
using Gathering.Application.Abstractions;
using Gathering.Application.Common.Validators;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Communities;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.Create;

public sealed class CreateSessionCommandHandler : ICommandHandler<CreateSessionCommand, Guid>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICommunityRepository _communityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSessionCommand> _validator;
    private readonly IImageStorageService _imageStorageService;

    public CreateSessionCommandHandler(
        ISessionRepository sessionRepository,
        ICommunityRepository communityRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateSessionCommand> validator,
        IImageStorageService imageStorageService)
    {
        _sessionRepository = sessionRepository;
        _communityRepository = communityRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<Guid>> HandleAsync(CreateSessionCommand request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure<Guid>(Error.Validation("CreateSession.ValidationFailed", errorMessages));
        }

        var communityExists = await _communityRepository.ExistsAsync(request.CommunityId, cancellationToken);

        if (!communityExists)
        {
            return Result.Failure<Guid>(CommunityError.NotFound);
        }

        // Handle image upload if provided
        string? imageUrl = null;
        if (request.ImageStream is not null && request.ImageFileName is not null && request.ImageContentType is not null)
        {
            // Validate image file
            var fileValidationResult = FileValidator.ValidateImageFile(
                request.ImageStream,
                request.ImageFileName,
                request.ImageContentType);

            if (fileValidationResult.IsFailure)
            {
                return Result.Failure<Guid>(fileValidationResult.Error);
            }

            // Upload image to Azure Blob Storage
            var uploadResult = await _imageStorageService.UploadImageAsync(
                request.ImageStream,
                request.ImageFileName,
                request.ImageContentType,
                "sessions",
                cancellationToken);

            if (uploadResult.IsFailure)
            {
                return Result.Failure<Guid>(uploadResult.Error);
            }

            imageUrl = uploadResult.Value;
        }

        var sessionResult = Session.Create(request.CommunityId, request.Title, request.Speaker, request.ScheduledAt, request.Description, imageUrl);

        if (sessionResult.IsFailure)
        {
            // If session creation failed and we uploaded an image, delete it
            if (imageUrl is not null)
            {
                await _imageStorageService.DeleteImageAsync(imageUrl, cancellationToken);
            }
            return Result.Failure<Guid>(sessionResult.Error);
        }

        _sessionRepository.Add(sessionResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(sessionResult.Value.Id);
    }
}
