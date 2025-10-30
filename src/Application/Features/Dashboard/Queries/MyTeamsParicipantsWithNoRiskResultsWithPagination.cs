using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class MyTeamsParicipantsWithNoRiskResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : MyTeamsParticipantsWithNoRiskResultsAdvancedFilter, IRequest<Result<PaginatedData<MyTeamsParticipantsWithNoRiskDto>>>
    {
        public MyTeamsParticipantsWithNoRiskResultsAdvancedSpecification Specification => new(this);
        public required MyTeamsParticipantsWithNoRiskGroupingType GroupingType { get; set; }
    }

    public enum MyTeamsParticipantsWithNoRiskGroupingType
    {
        User,
        Tenant
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<PaginatedData<MyTeamsParticipantsWithNoRiskDto>>>
    {
        public async Task<Result<PaginatedData<MyTeamsParticipantsWithNoRiskDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            PaginatedData<MyTeamsParticipantsWithNoRiskDto> data = await GetData(request, cancellationToken);
            return data;  
        }

        private async Task<PaginatedData<MyTeamsParticipantsWithNoRiskDto>> GetData(Query request, CancellationToken cancellationToken)
        {
            return request.GroupingType switch
            {
                MyTeamsParticipantsWithNoRiskGroupingType.User => await GetParticipantsWithNoRiskAggregateByUser(request, cancellationToken),
                MyTeamsParticipantsWithNoRiskGroupingType.Tenant => await GetParticipantsWithNoRiskAggregateByTenant(request, cancellationToken),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private async Task<PaginatedData<MyTeamsParticipantsWithNoRiskDto>> GetParticipantsWithNoRiskAggregateByTenant(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var groupedQuery =  from p in db.Participants.ApplySpecification(request.Specification)
                                join owner in db.Users on p.OwnerId equals owner.Id
                                join t in db.Tenants on owner.TenantId equals t.Id
                                join r in db.Risks on p.Id equals r.ParticipantId into riskGroup
                                from r in riskGroup.DefaultIfEmpty()
                                where r.Created == null
                                group p by new { t.Id, t.Name } into g
                                orderby g.Key.Name
                                select new MyTeamsParticipantsWithNoRiskDto
                                {
                                    Description = g.Key.Name!,
                                    Count = g.Count()
                                };

            var results = await groupedQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var count = await groupedQuery.CountAsync(cancellationToken);

            return new PaginatedData<MyTeamsParticipantsWithNoRiskDto>(
                results, count, request.PageNumber, request.PageSize);
        }

        private async Task<PaginatedData<MyTeamsParticipantsWithNoRiskDto>> GetParticipantsWithNoRiskAggregateByUser(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;

#pragma warning disable CS8602
            var query = from p in db.Participants.ApplySpecification(request.Specification)
                        join owner in db.Users on p.OwnerId equals owner.Id
                        join r in db.Risks on p.Id equals r.ParticipantId into riskGroup
                        from r in riskGroup.DefaultIfEmpty()
                        where r.Created == null
                        group p by new { owner.Id, owner.DisplayName} into g
                        orderby g.Key.DisplayName
                        select new MyTeamsParticipantsWithNoRiskDto
                        {
                            Description = g.Key.DisplayName,  
                            Count = g.Count()
                        };

            var results = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var count = await query.CountAsync(cancellationToken);

            return new PaginatedData<MyTeamsParticipantsWithNoRiskDto>(
                results, count, request.PageNumber, request.PageSize);
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(r => r.Keyword)
                    .Matches(ValidationConstants.Keyword)
                    .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));

                RuleFor(r => r.PageNumber)
                    .GreaterThan(0)
                    .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

                RuleFor(r => r.PageSize)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(ValidationConstants.MaximumPageSize)
                    .WithMessage(ValidationConstants.MaximumPageSizeMessage);                
            }
        }
    }
}