using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetCaseWorkload
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Query : IRequest<Result<CaseSummaryDto[]>>
    {
        public UserProfile? CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<CaseSummaryDto[]>>
    {
        public async Task<Result<CaseSummaryDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {

            
            var tenantId = $"{request.CurrentUser!.TenantId!}%";

            var results = await unitOfWork.DbContext.Database
                .SqlQuery<CaseSummaryDto>(
                    $@"SELECT 
            u.UserName,
            u.TenantName,
            p.EnrolmentStatus as EnrolmentStatusId, 
            l.Name as LocationName,
            COUNT(*) as [Count]
        FROM 
            Participant.Participant as p
        INNER JOIN
            [Configuration].Location as l ON p.CurrentLocationId = l.Id
        INNER JOIN
            [Identity].[User] as u ON p.OwnerId = u.Id
        WHERE
            u.TenantId LIKE {tenantId}  
        GROUP BY 
            u.UserName,
            u.TenantName,
            p.EnrolmentStatus,
            l.Name"
                ).ToArrayAsync(cancellationToken);

            return Result<CaseSummaryDto[]>.Success(results);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(q => q.CurrentUser)
                    .NotNull();
            }
        }
    }

}