using System.Reflection.Emit;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using DocumentFormat.OpenXml.Math;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetDashboard 
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<DashboardDto>>
    {
        public UserProfile? CurrentUser { get; set; } 
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<DashboardDto>>
    {
        public async Task<Result<DashboardDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = from p in unitOfWork.DbContext.Participants.AsNoTracking()
                where p.OwnerId == request.CurrentUser!.UserId
                group p by p.EnrolmentStatus
                into grp
                select new
                {
                    Status = grp.Key,
                    Count = grp.Count()
                };

            var results = await query.ToArrayAsync(cancellationToken);

            DashboardDto dto = new DashboardDto();
            foreach (var result in results)
            {
                if (result.Status == EnrolmentStatus.ApprovedStatus)
                {
                    dto.ApprovedCases = result.Count;
                    continue;
                }
                if (result.Status == EnrolmentStatus.IdentifiedStatus)
                {
                    dto.PendingCases = result.Count;
                    continue;
                }
                if(result.Status == EnrolmentStatus.EnrollingStatus)
                {
                    dto.ConfirmedCases = result.Count;
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

            return Result<DashboardDto>.Success(dto);

        }
    }

    public  class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(q => q.CurrentUser)
                .NotNull();
        }
    }

}

public class DashboardDto
{
    public int PendingCases { get; set; }
    public int ConfirmedCases { get; set; }
    public int CasesAtPqa { get; set; }
    public int CasesAtCfo { get; set; }
    public int ApprovedCases { get; set; }
    public int UnreadNotifications { get; set; }
}
