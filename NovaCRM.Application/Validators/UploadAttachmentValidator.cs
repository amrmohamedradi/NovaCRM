using FluentValidation;
using NovaCRM.Application.Features.Attachments.Commands;

namespace NovaCRM.Application.Validators;

public class UploadAttachmentValidator : AbstractValidator<UploadAttachmentCommand>
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "application/pdf",
        "image/png", "image/jpeg", "image/gif",
        "text/plain",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ];

    private const long MaxSizeBytes = 10 * 1024 * 1024;

    public UploadAttachmentValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("FileName is required.")
            .MaximumLength(255)
            .WithMessage("FileName must not exceed 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("ContentType is required.")
            .Must(ct => AllowedContentTypes.Contains(ct.ToLowerInvariant()))
            .WithMessage("File type not allowed. Permitted: PDF, PNG, JPEG, GIF, TXT, DOC, DOCX, XLS, XLSX.");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0)
            .WithMessage("File must not be empty.")
            .LessThanOrEqualTo(MaxSizeBytes)
            .WithMessage("File size must not exceed 10 MB.");

        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File stream is required.");
    }
}
