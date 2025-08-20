using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantsLatestEngagement
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<IEnumerable<ParticipantEngagementDto>>>
    {
        public bool JustMyCases { get; set; } = false;
    }

    private class Handler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService) : IRequestHandler<Query, Result<IEnumerable<ParticipantEngagementDto>>>
    {
        public async Task<Result<IEnumerable<ParticipantEngagementDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

#pragma warning disable CS8602, CS8604
            var query = 
                from participant in db.Participants
                join engagement in db.ParticipantEngagements
                    on participant.Id equals engagement.ParticipantId into leftJoin
                from engagement in leftJoin
                    .OrderByDescending(pe => pe.EngagedOn)
                    .ThenByDescending(pe => pe.CreatedOn)
                    .Take(1)
                    .DefaultIfEmpty()
                join supportWorker in db.Users on engagement.UserId equals supportWorker.Id
                where participant.Owner.TenantId.StartsWith(currentUserService.TenantId)
                where (request.JustMyCases && participant.Owner.Id == currentUserService.UserId) || true
                where participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                orderby engagement.CreatedOn descending
                select new ParticipantEngagementDto(
                    participant.Id,
                    $"{participant.FirstName} {participant.LastName}",
                    engagement.Category,
                    engagement.Description,
                    supportWorker.DisplayName,
                    engagement.EngagedOn);
#pragma warning restore CS8602, CS8604

            var engagements = await query.ToListAsync(cancellationToken);

            return Result<IEnumerable<ParticipantEngagementDto>>.Success(engagements);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {

        }
    }
}
