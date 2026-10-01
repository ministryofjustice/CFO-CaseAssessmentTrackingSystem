using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;

/// <summary>
/// Creates/updates HelpLinks from a resolved set of import decisions (see
/// <see cref="HelpLinkImportDecisionDto"/>), produced after the admin has reviewed the output of
/// <c>PreviewHelpLinksImportCommand</c> and chosen how to resolve any conflicts.
/// </summary>
[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class ApplyHelpLinksImportCommand : ICommand<Result<ApplyHelpLinksImportResultDto>>
{
    public List<HelpLinkImportDecisionDto> Decisions { get; set; } = new();
}
