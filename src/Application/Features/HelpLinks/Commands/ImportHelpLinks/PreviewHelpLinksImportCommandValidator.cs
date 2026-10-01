namespace Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;

public class PreviewHelpLinksImportCommandValidator : AbstractValidator<PreviewHelpLinksImportCommand>
{
    public PreviewHelpLinksImportCommandValidator() => RuleFor(x => x.Data).NotNull().NotEmpty();
}
