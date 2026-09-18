using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Labels.Commands.EditLabel;
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
            .Matches(ValidationConstants.Keyword)
            .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Name"));

        RuleFor(v => v.NewDescription)
            .Matches(ValidationConstants.Notes)
            .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));

        RuleFor(x => x.NewContractIds)
            .NotEmpty()
            .WithMessage("A label must be assigned to at least one contract");
    }
}
