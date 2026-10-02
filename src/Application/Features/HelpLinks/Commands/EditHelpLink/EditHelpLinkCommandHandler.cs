using Cfo.Cats.Application.Features.HelpLinks.Commands.EditHelpLink;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands;

public class EditHelpLinkCommandHandler(
        IHelpLinkRepository repository,
        IHelpLinkCounter helpLinkCounter) : ICommandHandler<EditHelpLinkCommand, Result>
{
    public async Task<Result> Handle(EditHelpLinkCommand request, CancellationToken cancellationToken)
    {
        var helpLink = await repository.GetByIdAsync(request.HelpLinkId);

        helpLink.Edit(
            request.NewTitle,
            request.NewDescription,
            request.NewUrls.Select(u => new HelpLinkUrlInput(u.Url, u.DisplayName)),
            request.NewPageKey,
            request.NewTabName,
            helpLinkCounter);

        return Result.Success();
    }
}
