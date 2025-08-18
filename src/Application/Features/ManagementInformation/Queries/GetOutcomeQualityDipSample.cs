using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetOutcomeQualityDipSample
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
    public class Query : IRequest<Result<DipSampleSummaryDto>>
    {
        public required Guid DipSampleId { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<DipSampleSummaryDto>>
    {
        public async Task<Result<DipSampleSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from sample in context.OutcomeQualityDipSamples
                join contract in context.Contracts on sample.ContractId equals contract.Id
                where sample.Id == request.DipSampleId
                select new DipSampleSummaryDto(contract.Description, sample.PeriodFrom, sample.ReviewedOn, sample.Status);

            var dipSample = await query.FirstOrDefaultAsync(cancellationToken);

            return dipSample switch
            {
                null => Result<DipSampleSummaryDto>.NotFound(),
                _ => dipSample
            };
        }
    }

    private class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.DipSampleId)
                .NotEmpty()
                .WithMessage(string.Format(ValidationConstants.GuidMessage, nameof(Query.DipSampleId)));
        }
    }

}