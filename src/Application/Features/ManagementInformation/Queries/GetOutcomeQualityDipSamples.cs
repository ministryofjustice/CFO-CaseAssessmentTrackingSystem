using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetOutcomeQualityDipSamples
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
    public class Query : IRequest<Result<DipSampleDto[]>>
    {
        public int Month { get; set; } = DateTime.Now.AddMonths(-4).Month;
        public int Year { get; set; } = DateTime.Now.AddMonths(-4).Year;
        public ContractDto? Contract { get; set; }
        public bool OnlyShowInProgress { get; set; } = false;
        public DateTime Period => new(Year, Month, 1);
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<DipSampleDto[]>>
    {
        public async Task<Result<DipSampleDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from sample in context.OutcomeQualityDipSamples
                join contract in context.Contracts on sample.ContractId equals contract.Id
                join u in context.Users on sample.ReviewedBy equals u.Id into uj
                from user in uj.DefaultIfEmpty()
                join dsp in context.OutcomeQualityDipSampleParticipants on sample.Id equals dsp.DipSampleId into
                    participants
                where sample.PeriodFrom == request.Period
                where request.Contract == null || request.Contract.Id == sample.ContractId
                where request.OnlyShowInProgress == false || sample.Status == DipSampleStatus.AwaitingReview
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
                    participants.Count(p => p.CsoReviewedOn.HasValue),
                    participants.Count(p => p.CpmReviewedOn.HasValue),
                    participants.Count(p => p.FinalReviewedOn.HasValue),
                    sample.ReviewedOn,
                    user.DisplayName,
                    (
                        from p in context.OutcomeQualityDipSampleParticipants
                        join u in context.Users on p.CsoReviewedBy equals u.Id
                        where p.DipSampleId == sample.Id && p.CsoReviewedBy != null
                        group u by u.DisplayName into g
                        select g.Key
                    ).ToArray(),
                    (
                        from p in context.OutcomeQualityDipSampleParticipants
                        join u in context.Users on p.CpmReviewedBy equals u.Id
                        where p.DipSampleId == sample.Id && p.CsoReviewedBy != null
                        group u by u.DisplayName into g
                        select g.Key
                    ).ToArray()
                    );

            var samples = await query
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return samples switch
            {
                { Length: 0 } => Result<DipSampleDto[]>.NotFound(),
                _ => Result<DipSampleDto[]>.Success(samples)
            };
        }
    }

    private class Validator : AbstractValidator<Query>
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