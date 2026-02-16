using FluentValidation;
using Gathering.Application.Abstractions;
using Gathering.Application.Common.Validators;
using Gathering.Domain.Abstractions;
using Gathering.Domain.Sessions;
using Gathering.SharedKernel;

namespace Gathering.Application.Sessions.Update;

public sealed class UpdateSessionCommandHandler : ICommandHandler<UpdateSessionCommand>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateSessionCommand> _validator;
    private readonly IImageStorageService _imageStorageService;

    public UpdateSessionCommandHandler(
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateSessionCommand> validator,
        IImageStorageService imageStorageService)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result> HandleAsync(UpdateSessionCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure(Error.Validation("UpdateSession.ValidationFailed", errorMessages));
        }

        var session = await _sessionRepository.GetByIdAsync(command.SessionId);

        if (session is null)
        {
            return Result.Failure(SessionError.NotFound);
        }

        // Handle image upload if a new image is provided
        string? newImageUrl = session.Image; // Keep existing image by default
        if (command.ImageStream is not null && command.ImageFileName is not null && command.ImageContentType is not null)
        {
            // Validate image file
            var fileValidationResult = FileValidator.ValidateImageFile(
                command.ImageStream,
                command.ImageFileName,
                command.ImageContentType);

            if (fileValidationResult.IsFailure)
            {
                return Result.Failure(fileValidationResult.Error);
            }

            // Upload new image to Azure Blob Storage
            var uploadResult = await _imageStorageService.UploadImageAsync(
                command.ImageStream,
                command.ImageFileName,
                command.ImageContentType,
                "sessions",
                cancellationToken);

            if (uploadResult.IsFailure)
            {
                return Result.Failure(uploadResult.Error);
            }

            newImageUrl = uploadResult.Value;

            // Delete old image if it exists and is a blob URL (not an external URL)
            if (!string.IsNullOrEmpty(session.Image) && session.Image.Contains("blob.core.windows.net", StringComparison.OrdinalIgnoreCase))
            {
                await _imageStorageService.DeleteImageAsync(session.Image, cancellationToken);
            }
        }

        var updateResult = session.Update(
            command.Title,
            command.Description,
            command.Speaker,
            command.Schedule,
            newImageUrl);

        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        session.UpdateState(command.State);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}