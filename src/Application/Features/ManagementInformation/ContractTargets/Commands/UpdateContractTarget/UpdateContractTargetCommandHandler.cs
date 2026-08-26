using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.Commands.UpdateContractTarget;

public class UpdateContractTargetCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateContractTargetCommand, Result>
{
    public async Task<Result> Handle(UpdateContractTargetCommand request, CancellationToken cancellationToken)
    {
        var target = await unitOfWork.DbContext.ContractTargets
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (target is null)
        {
            return Result.Failure($"No contract target found with id '{request.Id}'.");
        }

        target.Prison = request.Prison;
        target.Community = request.Community;
        target.Wings = request.Wings;
        target.Hubs = request.Hubs;
        target.PreReleaseSupport = request.PreReleaseSupport;
        target.ThroughTheGate = request.ThroughTheGate;
        target.SupportWork = request.SupportWork;
        target.HumanCitizenship = request.HumanCitizenship;
        target.CommunityAndSocial = request.CommunityAndSocial;
        target.Interventions = request.Interventions;
        target.Employment = request.Employment;
        target.TrainingAndEducation = request.TrainingAndEducation;

        await unitOfWork.DbContext.InsertOutboxMessage(new TargetsChangedIntegrationEvent());

        return Result.Success();
    }
}
