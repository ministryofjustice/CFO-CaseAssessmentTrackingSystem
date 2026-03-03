using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public class EditLabelCommandValidator : AbstractValidator<EditLabelCommand>
{
    public EditLabelCommandValidator()
    {
        RuleFor(x => x.NewName)
            .NotEmpty()
            .MinimumLength(LabelConstants.NameMinimumLength)
            .MaximumLength(LabelConstants.NameMaximumLength);

        RuleFor(x => x.NewDescription)
            .NotEmpty()
            .MinimumLength(LabelConstants.DescriptionMinimumLength)
            .MaximumLength(LabelConstants.DescriptionMaximumLength);

        RuleFor(v => v.NewName)
            .Matches(ValidationConstants.LettersSpacesUnderscores)
            .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Name"));

        RuleFor(v => v.NewDescription)
            .Matches(ValidationConstants.Notes)
            .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));
    }
}
