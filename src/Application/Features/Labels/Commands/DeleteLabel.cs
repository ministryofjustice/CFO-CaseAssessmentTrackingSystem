using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Labels.Rules;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public static class DeleteLabel
{
    [RequestAuthorize(Policy = SecurityPolicies.ManageLabels)]
    public class Command : IRequest<Result>
    {
        public required UserProfile UserProfile { get; set; }   
        public required LabelId LabelId { get; set; }
    }

    public class Handler(
        ILabelRepository repository,
        ILabelCounter labelCounter) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command request, 
            CancellationToken cancellationToken)
        {
            var label = await repository.GetByIdAsync(request.LabelId);
            if (label is null)
            {
                return Result.Failure("Label not found.");
            }

            label.Delete(request.UserProfile.AsDomainUser(), labelCounter);
            return Result.Success();
        }
    }
}