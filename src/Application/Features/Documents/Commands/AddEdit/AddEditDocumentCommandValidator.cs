namespace Cfo.Cats.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommandValidator : AbstractValidator<AddEditDocumentCommand>
{
    public AddEditDocumentCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotNull()
            .MaximumLength(256)
            .NotEmpty();
        RuleFor(v => v.DocumentType)
            .NotNull();
        RuleFor(v => v.Description)
            .MaximumLength(256);
        RuleFor(v => v.UploadRequest)
            .NotNull()
            .When(x => x.Id == Guid.Empty);
    }
}