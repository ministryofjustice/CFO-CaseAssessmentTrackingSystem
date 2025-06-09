using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetCumulativeFigures
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : IRequest<Result<CumulativeFiguresDto>>
    {
        public required DateOnly EndDate { get; set; }
        public required string? ContractId { get; set; }
        
        public required UserProfile CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : IRequestHandler<Query, Result<CumulativeFiguresDto>>
    {
        public async Task<Result<CumulativeFiguresDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            DateOnly startDate = new DateOnly(2025, 1, 1);
            DateOnly endDate = request.EndDate;

            var results = await unitOfWork.DbContext.Database
                .SqlQuery<Actuals>($"SELECT * FROM mi.GetCumulativeTotals({startDate}, {endDate})")
                .ToArrayAsync(cancellationToken);

            Actuals? actuals;
            ContractTargetDto target = GetTargets(request);
            
            if (request.ContractId is not null)
            {
                // user has asked for one contract, so return that.
                actuals = results.First(c => c.contract_id == request.ContractId);
                
            }
            else
            {
                actuals =  results
                    .Where(c => request.CurrentUser.Contracts.Contains(c.contract_id))
                    .Aggregate(
                    new Actuals("All", "All", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                    (acc, x) => acc with
                    {
                        custody_enrolments = acc.custody_enrolments + x.custody_enrolments,
                        community_enrolments = acc.community_enrolments + x.community_enrolments,
                        wing_inductions = acc.wing_inductions + x.wing_inductions,
                        hub_inductions = acc.hub_inductions + x.hub_inductions,
                        prerelease_support = acc.prerelease_support + x.prerelease_support,
                        ttg = acc.ttg + x.ttg,
                        support_work = acc.support_work + x.support_work,
                        human_citizenship = acc.human_citizenship + x.human_citizenship,
                        community_and_social = acc.community_and_social + x.community_and_social,
                        isws = acc.isws + x.isws,
                        employment = acc.employment + x.employment,
                        education = acc.education + x.education
                    });
            }

            
            
            return new CumulativeFiguresDto(actuals, target);

        }
        
        private ContractTargetDto GetTargets(Query request)
        {
            //start date is fixed
            var startDate = new DateOnly(2025, 1, 1);

            var monthlyDates = Enumerable.Range(0, int.MaxValue)
                .Select(i => startDate.AddMonths(i))
                .TakeWhile(date => date <= request.EndDate)
                .ToArray();

            // get all contracts that apply

            string[] contracts = request.ContractId is null ? 
                request.CurrentUser.Contracts
                : [request.ContractId];
        
            // iterate over every contract and get the targets for the months
            ContractTargetDto target = new ContractTargetDto("", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            foreach (var contract in contracts)
            {
                foreach (var month in monthlyDates)
                {
                    target += targetsProvider.GetTargetById(contract, month.Month, month.Year);
                }
            }

            return target;


        }

        
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(s => s.EndDate)
                .GreaterThan(new DateOnly(2024, 12, 31))
                .WithMessage("End date cannot before 01 January 2025");

            // if the contract id has been passed, then
            // it must be on that's valid for the current user
            When(s => s.ContractId is not null, () => {
                RuleFor(s => s)
                    .Must(query => query.CurrentUser.Contracts.Any(c => c == query.ContractId));
            });

        }
    }
}