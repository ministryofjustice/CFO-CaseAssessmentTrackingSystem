using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;

public class ApplyHelpLinksImportCommandValidator : AbstractValidator<ApplyHelpLinksImportCommand>
{
    public ApplyHelpLinksImportCommandValidator()
    {
        RuleFor(x => x.Decisions)
            .NotEmpty()
            .WithMessage("At least one HelpLink must be selected to import.");

        RuleForEach(x => x.Decisions)
            .SetValidator(new HelpLinkImportDecisionDtoValidator());
    }
}

public class HelpLinkImportDecisionDtoValidator : AbstractValidator<HelpLinkImportDecisionDto>
{
    public HelpLinkImportDecisionDtoValidator()
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
