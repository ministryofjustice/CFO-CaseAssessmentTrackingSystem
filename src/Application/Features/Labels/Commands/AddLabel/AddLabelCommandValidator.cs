using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Labels.Commands.AddLabel;
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
            .Matches(ValidationConstants.Keyword)
            .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Name"));

        RuleFor(v => v.Description)
            .Matches(ValidationConstants.Notes)
            .WithMessage(string.Format(ValidationConstants.NotesMessage, "Description"));

        RuleFor(x => x.ContractIds)
            .NotEmpty()
            .WithMessage("A label must be assigned to at least one contract");
    }
}
