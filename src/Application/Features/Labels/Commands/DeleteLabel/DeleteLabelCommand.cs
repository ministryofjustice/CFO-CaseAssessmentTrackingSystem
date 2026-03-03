using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

[RequestAuthorize(Policy = SecurityPolicies.ManageLabels)]
public class DeleteLabelCommand : IRequest<Result>
{
    public required UserProfile UserProfile { get; set; }
    public required LabelId LabelId { get; set; }
}
