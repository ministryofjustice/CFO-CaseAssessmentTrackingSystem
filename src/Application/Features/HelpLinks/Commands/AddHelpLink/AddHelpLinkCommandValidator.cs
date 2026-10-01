using Cfo.Cats.Application.Features.HelpLinks.Commands.AddHelpLink;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands;

public class AddHelpLinkCommandValidator : AbstractValidator<AddHelpLinkCommand>
{
    public AddHelpLinkCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(HelpLinkConstants.TitleMaximumLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(HelpLinkConstants.DescriptionMaximumLength);

        RuleFor(x => x.Urls)
            .NotEmpty()
            .WithMessage("At least one Url must be provided.");

        RuleForEach(x => x.Urls)
            .SetValidator(new HelpLinkUrlDtoValidator());

        RuleFor(x => x.PageKey)
            .NotEmpty()
            .MaximumLength(HelpLinkConstants.PageKeyMaximumLength);

        RuleFor(x => x.TabName)
            .MaximumLength(HelpLinkConstants.TabNameMaximumLength);
    }
}
