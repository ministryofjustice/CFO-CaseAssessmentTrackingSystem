using Cfo.Cats.Application.Features.HelpLinks.Commands.EditHelpLink;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands;

public class EditHelpLinkCommandValidator : AbstractValidator<EditHelpLinkCommand>
{
    public EditHelpLinkCommandValidator()
    {
        RuleFor(x => x.HelpLinkId)
            .NotEmpty();

        RuleFor(x => x.NewTitle)
            .NotEmpty()
            .MaximumLength(HelpLinkConstants.TitleMaximumLength);

        RuleFor(x => x.NewDescription)
            .NotEmpty()
            .MaximumLength(HelpLinkConstants.DescriptionMaximumLength);

        RuleFor(x => x.NewUrls)
            .NotEmpty()
            .WithMessage("At least one Url must be provided.");

        RuleForEach(x => x.NewUrls)
            .SetValidator(new HelpLinkUrlDtoValidator());

        RuleFor(x => x.NewPageKey)
            .NotEmpty()
            .MaximumLength(HelpLinkConstants.PageKeyMaximumLength);

        RuleFor(x => x.NewTabName)
            .MaximumLength(HelpLinkConstants.TabNameMaximumLength);
    }
}
