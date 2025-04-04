using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using DocumentFormat.OpenXml.Drawing;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetRiskDueAggregate
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Query : IRequest<Result<RiskDueAggregateDto[]>>
    {
        public required UserProfile CurrentUser { get; set; }

        public required RiskAggregateGroupingType GroupingType { get; set; }
    }

    public enum RiskAggregateGroupingType
    {
        User,
        Tenant
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<RiskDueAggregateDto[]>>
    {
        public async Task<Result<RiskDueAggregateDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            RiskDueAggregateDto[] data = await GetData(request);
            return data;
        }

        private async Task<RiskDueAggregateDto[]> GetData(Query request)
        {
            return request.GroupingType switch
            {
                RiskAggregateGroupingType.User => await GetRiskAggregateByUser(request),
                RiskAggregateGroupingType.Tenant => await GetRiskAggregateByTenant(request),
                _ => throw new ArgumentOutOfRangeException()
            };

        }

        private async Task<RiskDueAggregateDto[]> GetRiskAggregateByUser(Query request)
        {
            var today = DateTime.Now.Date;
            var twoWeeks = today.AddDays(14);

            var context = unitOfWork.DbContext;

            var query = from user in context.Users

                join overdueRiskGroup in
                    (from user in context.Users
                        join participant in context.Participants on user.Id equals participant.OwnerId
                        where participant.RiskDue < today
                              && participant.EnrolmentStatus == 3
                        group user by user.Id into g
                        select new
                        {
                            UserId = g.Key,
                            Records = (int?)g.Count()  
                        })
                    on user.Id equals overdueRiskGroup.UserId into overdueRiskJoin
                from overdueRisk in overdueRiskJoin.DefaultIfEmpty()

                join upcomingRiskGroup in
                    (from user in context.Users
                        join participant in context.Participants on user.Id equals participant.OwnerId
                        where participant.RiskDue >= today && participant.RiskDue <= twoWeeks
                                                           && participant.EnrolmentStatus == 3
                        group user by user.Id into g
                        select new
                        {
                            UserId = g.Key,
                            Records = (int?)g.Count()  
                        })
                    on user.Id equals upcomingRiskGroup.UserId into upcomingRiskJoin
                from upcomingRisk in upcomingRiskJoin.DefaultIfEmpty()
                where user.TenantId!.StartsWith(request.CurrentUser.TenantId!)
                &&   overdueRisk.Records > 0 || upcomingRisk.Records > 0
                        orderby user.DisplayName
                select new RiskDueAggregateDto(user.DisplayName!, overdueRisk.Records ?? 0, upcomingRisk.Records ?? 0);

            return await query.ToArrayAsync();

        }
        private async Task<RiskDueAggregateDto[]> GetRiskAggregateByTenant(Query request)
        {
            var today = DateTime.Now.Date;
            var twoWeeks = today.AddDays(14);

            var context = unitOfWork.DbContext;

            var query = from tenant in context.Tenants
                join overdueRiskGroup in
                    (from user in context.Users
                        join participant in context.Participants on user.Id equals participant.OwnerId
                        where participant.RiskDue < today
                              && participant.EnrolmentStatus == 3
                        group user by user.TenantId into g
                        select new
                        {
                            TenantId = g.Key,
                            Records = (int?)g.Count()
                        })
                    on tenant.Id equals overdueRiskGroup.TenantId into overdueRiskJoin
                from overdueRisk in overdueRiskJoin.DefaultIfEmpty()

                join upcomingRiskGroup in
                    (from user in context.Users
                        join participant in context.Participants on user.Id equals participant.OwnerId
                        where participant.RiskDue >= today && participant.RiskDue <= twoWeeks
                                                           && participant.EnrolmentStatus == 3
                        group user by user.TenantId into g
                        select new
                        {
                            TenantId = g.Key,
                            Records = (int?)g.Count()
                        })
                    on tenant.Id equals upcomingRiskGroup.TenantId into upcomingRiskJoin
                from upcomingRisk in upcomingRiskJoin.DefaultIfEmpty()
                where tenant.Id.StartsWith(request.CurrentUser.TenantId!)
                    && overdueRisk.Records > 0 || upcomingRisk.Records > 0
                orderby tenant.Id
                select new RiskDueAggregateDto(tenant.Name!, overdueRisk.Records ?? 0, upcomingRisk.Records ?? 0);
       
            return await query.ToArrayAsync();
        }

    }
}