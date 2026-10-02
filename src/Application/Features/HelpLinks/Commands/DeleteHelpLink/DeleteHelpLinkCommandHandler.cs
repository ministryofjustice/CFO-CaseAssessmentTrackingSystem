using Cfo.Cats.Application.Features.HelpLinks.Commands.DeleteHelpLink;
using Cfo.Cats.Domain.HelpLinks;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands;

public class DeleteHelpLinkCommandHandler(IHelpLinkRepository repository) : ICommandHandler<DeleteHelpLinkCommand, Result>
{
    public async Task<Result> Handle(DeleteHelpLinkCommand request, CancellationToken cancellationToken)
    {
        var helpLink = await repository.GetByIdAsync(request.HelpLinkId);
        helpLink.Delete();
        return Result.Success();
    }
}
