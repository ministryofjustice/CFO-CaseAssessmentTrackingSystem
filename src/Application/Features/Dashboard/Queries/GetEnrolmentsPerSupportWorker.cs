using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetEnrolmentsPerSupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<EnrolmentsPerSupportWorkerDto>>
    {
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<EnrolmentsPerSupportWorkerDto>>
    {
        public async Task<Result<EnrolmentsPerSupportWorkerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) == false)
            {
                return await GetEnrolmentsByUserId(request, cancellationToken);
            }
            if (string.IsNullOrWhiteSpace(request.TenantId) == false)
            {
                return await GetEnrolmentsByTenantId(request, cancellationToken);
            }
            return Result<EnrolmentsPerSupportWorkerDto>.Failure("Invalid request: UserId or TenantId must be provided."); 
            
        }

        private async Task<Result<EnrolmentsPerSupportWorkerDto>> GetEnrolmentsByUserId(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from mi in context.EnrolmentPayments
                        join l in context.Locations on mi.LocationId equals l.Id
                        where mi.EligibleForPayment
                        && mi.SupportWorker == request.UserId
                        && mi.Approved >= request.StartDate
                        && mi.Approved <= request.EndDate
                        group l by l into grp
                        select new LocationDetail
                            (
                                grp.Key.Name,
                                grp.Key.LocationType,
                                grp.Count()
                            );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new EnrolmentsPerSupportWorkerDto(results);
        }
        private async Task<Result<EnrolmentsPerSupportWorkerDto>> GetEnrolmentsByTenantId(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query = from mi in context.EnrolmentPayments
                        join l in context.Locations on mi.LocationId equals l.Id
                        join u in context.Users on mi.SupportWorker equals u.Id
                        where mi.EligibleForPayment
                        && u.TenantId!.StartsWith(request.TenantId!)
                        && mi.Approved >= request.StartDate
                        && mi.Approved <= request.EndDate
                        group l by l into grp
                        select new LocationDetail
                            (
                                grp.Key.Name,
                                grp.Key.LocationType,
                                grp.Count()
                            );

            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new EnrolmentsPerSupportWorkerDto(results);
        }

    }

    public record EnrolmentsPerSupportWorkerDto
    {
        public EnrolmentsPerSupportWorkerDto(LocationDetail[] details) 
        {
            Details = details;
            Custody = details.Where(d => d.LocationType.IsCustody).Sum(d => d.Count);
            Community = details.Where(d => d.LocationType.IsCommunity).Sum(d => d.Count);
        }

        public LocationDetail[] Details { get; }

        public int Custody { get; }

        public int Community { get; }

    }

    public record LocationDetail (string LocationName, LocationType LocationType, int Count);

}