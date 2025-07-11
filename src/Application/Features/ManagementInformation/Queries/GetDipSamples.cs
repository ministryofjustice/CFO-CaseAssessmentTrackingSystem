using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetDipSamples
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : IRequest<Result<IEnumerable<DipSampleDto>>>
    {
        public required int Month { get; set; }
        public required int Year { get; set; }

        public DateTime Period => new (Year, Month, day: 1);
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IEnumerable<DipSampleDto>>>
    {
        public async Task<Result<IEnumerable<DipSampleDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from sample in context.DipSamples
                join contract in context.Contracts on sample.ContractId equals contract.Id
                join u in context.Users on sample.CompletedBy equals u.Id into uj
                from user in uj.DefaultIfEmpty()
                join dsp in context.DipSampleParticipants on sample.Id equals dsp.DipSampleId into participants
                where sample.PeriodFrom == request.Period
                orderby contract.Description
                select new DipSampleDto(
                    sample.Id, 
                    contract.Description, 
                    sample.Status, 
                    sample.PeriodFrom,
                    sample.CreatedOn,
                    participants.Count(),
                    sample.ScoreAvg, 
                    sample.CompletedOn, 
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
