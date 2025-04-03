using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetRiskDueDashboard
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<RiskDueDto[]>
    {
        public required string UserId { get; set; }

        public int FuturesDays { get; set; } = 14;

        public bool ApprovedOnly { get; set; } = true;

    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, RiskDueDto[]>
    {
        public async Task<RiskDueDto[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var maxDate = DateTime.Today.AddDays(request.FuturesDays);

            var query = from p in unitOfWork.DbContext.Participants
                join r in unitOfWork.DbContext.Risks
                        .GroupBy(r => r.ParticipantId)
                        .Select(g => new { ParticipantId = g.Key, Completed = g.Max(r => r.Completed) })
                    on p.Id equals r.ParticipantId into risks
                from risk in risks.DefaultIfEmpty()
                where 
                        p.OwnerId! == request.UserId && p.RiskDue <= maxDate
                        && ( request.ApprovedOnly == false || request.ApprovedOnly && p.EnrolmentStatus == EnrolmentStatus.ApprovedStatus.Value)
                orderby p.RiskDue
                select new RiskDueDto(p.Id, p.FirstName, p.LastName, p.RiskDue!.Value, risk == null ? null : risk.Completed, p.EnrolmentStatus!);


            return await query.ToArrayAsync(cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull();

            RuleFor(x => x.UserId)
                .Matches(ValidationConstants.Guid)
                .WithMessage(string.Format(ValidationConstants.GuidMessage, "UserId"));

            RuleFor(x => x.FuturesDays)
                .GreaterThan(0);

            RuleFor(x => x.FuturesDays)
                .LessThanOrEqualTo(365);

        }
    }

}