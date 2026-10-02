using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands;

public class HelpLinkUrlDtoValidator : AbstractValidator<HelpLinkUrlDto>
{
    public HelpLinkUrlDtoValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .MaximumLength(HelpLinkConstants.UrlMaximumLength)
            .Must(UrlValidator.IsHttpOrHttpsUrl)
            .WithMessage("Url must be a valid, absolute url.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(HelpLinkConstants.DisplayNameMaximumLength);
    }
}
