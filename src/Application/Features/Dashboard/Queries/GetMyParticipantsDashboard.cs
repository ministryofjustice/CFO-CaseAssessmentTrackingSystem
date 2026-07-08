using System.Reflection.Emit;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Newtonsoft.Json;
using Sentry.Extensibility;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetMyParticipantsDashboard 
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<ParticipantCountSummaryDto>>
    {
        public required UserProfile CurrentUser { get; set; } 
        public required bool IncludeTeams { get; set; }    
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ParticipantCountSummaryDto>>
    {
        public async Task<Result<ParticipantCountSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {

            var query = from p in unitOfWork.DbContext
                                .Participants.AsNoTracking()
                        select p;

            if(request.IncludeTeams is false)
            {
                query = from p in query 
                        where p.OwnerId == request.CurrentUser.UserId
                        select p;
            }
            else
            {
                query = from p in query
                        where p.Owner!.TenantId!.StartsWith(request.CurrentUser.TenantId!)
                        select p;
            }

            var groupQuery = from p in query
                group p by p.EnrolmentStatus
                into grp
                select new
                {
                    Status = grp.Key,
                    Count = grp.Count()
                };

            var results = await groupQuery.ToArrayAsync(cancellationToken);

            ParticipantCountSummaryDto dto = new ParticipantCountSummaryDto();
            foreach (var result in results)
            {
                if (result.Status == EnrolmentStatus.ApprovedStatus)
                {
                    dto.ApprovedCases = result.Count;
                    continue;
                }
                if (result.Status == EnrolmentStatus.IdentifiedStatus)
                {
                    dto.IdentifiedCases = result.Count;
                    continue;
                }
                if(result.Status == EnrolmentStatus.EnrollingStatus)
                {
                    dto.EnrollingCases = result.Count;
                    continue;
                }
                if (result.Status == EnrolmentStatus.SubmittedToProviderStatus)
                {
                    dto.CasesAtPqa = result.Count;
                    continue;
                }
                if (result.Status == EnrolmentStatus.SubmittedToAuthorityStatus)
                {
                    dto.CasesAtCfo = result.Count;
                }
            }

            return Result<ParticipantCountSummaryDto>.Success(dto);

        }
    }

    public  class Validator : AbstractValidator<Query>
    {
        public Validator() =>
            RuleFor(q => q.CurrentUser)
                .NotNull();
    }

}