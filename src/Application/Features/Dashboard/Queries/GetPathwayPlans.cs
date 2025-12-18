using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetPathwayPlans
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<PathwayPlanDto>>
    {
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PathwayPlanDto>>
    {
        public async Task<Result<PathwayPlanDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var baseQuery = from p in context.Participants 
                            join pp in context.PathwayPlans on p.Id equals pp.ParticipantId
                            join u in context.Users on p.OwnerId equals u.Id
                            join pcl in context.Locations on p.CurrentLocation.Id equals pcl.Id
                where p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                select new {
                    PathwayPlanId = pp.Id, 
                    ParticipantId = p.Id, 
                    ParticipantName = p.FirstName + " " + p.LastName,
                    EnrolmentStatus = p.EnrolmentStatus,
                    OwnerId = u.Id,
                    TenantId = u.TenantId,
                    ParticipantCurrentLocation = pcl.Name,
                    ParticipantCurrentLocationType = pcl.LocationType
                };
                
            // Checks and applies filter based on UserId or TenantId else throws exception
            baseQuery = request switch
            {
                { UserId: var userId } when !string.IsNullOrWhiteSpace(userId)
                    => baseQuery.Where(x => x.OwnerId == userId),
    
                { TenantId: var tenantId } when !string.IsNullOrWhiteSpace(tenantId)
                    => baseQuery.Where(x => x.TenantId!.StartsWith(tenantId)),

                _ => throw new ArgumentException("Invalid request: UserId or TenantId must be provided.")
            };

            var query = from data in baseQuery
                join pp in context.PathwayPlans on data.PathwayPlanId equals pp.Id
                from review in pp.PathwayPlanReviews.OrderByDescending(r => r.ReviewDate).Take(1).DefaultIfEmpty()
                join u in context.Users on review.CreatedBy equals u.Id into users
                from reviewUser in users.DefaultIfEmpty()
                join l in context.Locations on review.LocationId equals l.Id into locations
                from reviewLocation in locations.DefaultIfEmpty()
                select new PathwayPlanReviewTabularData
                {
                    ParticipantId = data.ParticipantId,
                    ParticipantName = data.ParticipantName,
                    ParticipantCurrentLocation = data.ParticipantCurrentLocation,
                    ParticipantCurrentLocationType = data.ParticipantCurrentLocationType ?? LocationType.Unknown,
                    EnrolmentStatus = data.EnrolmentStatus,
                    ReviewLocation = reviewLocation != null ? reviewLocation.Name : "Unknown",
                    ReviewedBy = reviewUser != null ? reviewUser.DisplayName : null,
                    ReviewDate = review != null ? review.ReviewDate : null,
                    ReviewNotes = review != null ? (review.Review ?? "").Replace("\r", " ").Replace("\n", " ") : "",
                    ReviewReason = review != null ? review.ReviewReason : PathwayPlanReviewReason.Default
                };
                
            var results = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return new PathwayPlanDto(results);
        }

    }

    public record PathwayPlanDto
    {
        public PathwayPlanDto(PathwayPlanReviewTabularData[] tabularData) 
        {
            TabularData = tabularData;

            ChartData = tabularData
            .GroupBy(td => new { td.ParticipantCurrentLocation, td.ParticipantCurrentLocationType })
            .OrderBy(g => g.Key.ParticipantCurrentLocation)
            .ThenBy(g => g.Key.ParticipantCurrentLocationType)
            .Select(g => new LocationDetail(
                g.Key.ParticipantCurrentLocation!,
                g.Key.ParticipantCurrentLocationType!,
                g.Count(),
                g.Count(d => d.IsOverdue)
            )).ToArray();

            Custody = ChartData.Where(d => d.LocationType?.IsCustody == true).Sum(d => d.TotalCount);
            Community = ChartData.Where(d => d.LocationType?.IsCommunity == true).Sum(d => d.TotalCount);
            TotalReviewed = tabularData.Count(d => d.IsOverdue == false);
            TotalOverdueCount = tabularData.Count(d => d.IsOverdue == true);
        }

        public PathwayPlanReviewTabularData[] TabularData { get; }
        public LocationDetail[] ChartData { get; }

        public int Custody { get; }

        public int Community { get; }

        public int TotalReviewed { get; }
        public int TotalOverdueCount { get; }
    }
    public record PathwayPlanReviewTabularData
    {
        public string? ParticipantCurrentLocation { get; set; }
        public LocationType? ParticipantCurrentLocationType { get; set; }
        public string? ParticipantId { get; set; }
        public string? ParticipantName { get; set; }
        public EnrolmentStatus? EnrolmentStatus { get; set; } 
        public DateTime? ReviewDate { get; set; }
        public bool IsOverdue => 
            ReviewDate is null || (DueDate is DateTime due && due < DateTime.UtcNow);
        public DateTime? DueDate => ReviewDate?.AddMonths(3);
        public string? ReviewedBy { get; set; }
        public string? ReviewNotes { get; set; }
        public PathwayPlanReviewReason? ReviewReason { get; set; }
        public string? ReviewLocation { get; set; }

    }
   public record LocationDetail(string LocationName, LocationType? LocationType, int TotalCount, int OverdueCount = 0);

}