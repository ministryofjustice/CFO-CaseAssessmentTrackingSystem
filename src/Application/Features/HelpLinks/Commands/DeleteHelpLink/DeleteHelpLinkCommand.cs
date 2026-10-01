using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.DeleteHelpLink;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class DeleteHelpLinkCommand : ICommand<Result>
{
    public required Guid HelpLinkId { get; set; }
}
