using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetOutcomeQualityDipSamples
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
    public class Query : IRequest<Result<IEnumerable<DipSampleDto>>>
    {
        public int Month { get; set; } = DateTime.Now.AddMonths(-4).Month;
        public int Year { get; set; } = DateTime.Now.AddMonths(-4).Year;
        public ContractDto? Contract { get; set; }
        public bool OnlyShowInProgress { get; set; } = false;
        public DateTime Period => new (Year, Month, day: 1);
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IEnumerable<DipSampleDto>>>
    {
        public async Task<Result<IEnumerable<DipSampleDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from sample in context.OutcomeQualityDipSamples
                join contract in context.Contracts on sample.ContractId equals contract.Id
                join u in context.Users on sample.ReviewedBy equals u.Id into uj
                from user in uj.DefaultIfEmpty()
                join dsp in context.OutcomeQualityDipSampleParticipants on sample.Id equals dsp.DipSampleId into participants
                where sample.PeriodFrom == request.Period
                where request.Contract == null || request.Contract.Id == sample.ContractId
                where request.OnlyShowInProgress == false || sample.Status == DipSampleStatus.InProgress
                orderby contract.Description
                select new DipSampleDto(
                    sample.Id, 
                    contract.Description, 
                    sample.Status, 
                    sample.PeriodFrom,
                    sample.CreatedOn,
                    participants.Count(),
                    sample.CsoScore, 
                    sample.CpmScore, 
                    sample.FinalScore,
                    sample.CsoPercentage,
                    sample.CpmPercentage,
                    sample.FinalPercentage,
                    participants.Where(p => p.CsoReviewedOn.HasValue).Count(),
                    sample.ReviewedOn, 
                    user.DisplayName);

            var samples = await query
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if(samples is not { Count: > 0 })
            {
                return Result<IEnumerable<DipSampleDto>>.Failure($"No dip samples have been generated for {request.Period:MMM yyyy}");
            }

            return Result<IEnumerable<DipSampleDto>>.Success(samples);
        }
    }

    class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(q => q.Month)
                    .ExclusiveBetween(1, 12)
                    .WithMessage("Invalid month");

                RuleFor(q => q.Year)
                    .LessThanOrEqualTo(DateTime.Today.Year)
                    .WithMessage("Cannot be in the future");
            });
        }
    }
}
