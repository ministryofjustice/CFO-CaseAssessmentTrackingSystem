using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public class AddLabelCommandValidator : AbstractValidator<AddLabelCommand>
{
    public AddLabelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(LabelConstants.NameMinimumLength)
            .MaximumLength(LabelConstants.NameMaximumLength);

        RuleFor(x => x.Description)
            .MinimumLength(LabelConstants.DescriptionMinimumLength)
            .MaximumLength(LabelConstants.DescriptionMaximumLength);

        RuleFor(v => v.Name)
            .Matches(ValidationConstants.LettersSpacesUnderscores)
            .WithMessage(string.Format(ValidationConstants.LettersSpacesUnderscoresMessage, "Name"));

        RuleFor(v => v.Description)
            .Matches(ValidationConstants.Notes)
            .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));
    }
}
