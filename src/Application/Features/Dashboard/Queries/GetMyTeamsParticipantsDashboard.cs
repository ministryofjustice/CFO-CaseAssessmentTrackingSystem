using System.Reflection.Emit;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using DocumentFormat.OpenXml.Math;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetMyTeamsParticipantsDashboard 
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<ParticipantCountSummaryDto>>
    {
        public UserProfile? CurrentUser { get; set; } 
        public string? TenantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ParticipantCountSummaryDto>>
    {
        public async Task<Result<ParticipantCountSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = from p in unitOfWork.DbContext
                    .Participants.AsNoTracking()
                where
                   p.Owner!.TenantId!.StartsWith(request.CurrentUser!.TenantId!)
                group p by p.EnrolmentStatus
                into grp
                select new
                {
                    Status = grp.Key,
                    Count = grp.Count()
                };

            var results = await query.ToArrayAsync(cancellationToken);

            var dto = new ParticipantCountSummaryDto();
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
        public Validator()
        {
            RuleFor(q => q.CurrentUser)
                .NotNull();

            RuleFor(q => q.CurrentUser!.AssignedRoles)
                .NotEmpty();

        }
    }

}