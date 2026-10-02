using Cfo.Cats.Application.Features.HelpLinks.Commands.AddHelpLink;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands;

public class AddHelpLinkCommandHandler(
        IHelpLinkRepository repository,
        IHelpLinkCounter helpLinkCounter) : ICommandHandler<AddHelpLinkCommand, Result>
{
    public async Task<Result> Handle(AddHelpLinkCommand request, CancellationToken cancellationToken)
    {
        var helpLink = HelpLink.Create(
            request.Title,
            request.Description,
            request.Urls.Select(u => new HelpLinkUrlInput(u.Url, u.DisplayName)),
            request.PageKey,
            request.TabName,
            helpLinkCounter);

        await repository.AddAsync(helpLink);
        return Result.Success();
    }
}
