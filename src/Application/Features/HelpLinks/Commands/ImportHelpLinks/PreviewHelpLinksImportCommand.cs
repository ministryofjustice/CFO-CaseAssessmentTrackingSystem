using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class PreviewHelpLinksImportCommand(byte[] data) : ICommand<Result<HelpLinkImportPreviewItemDto[]>>
{
    public byte[] Data { get; set; } = data;
}
