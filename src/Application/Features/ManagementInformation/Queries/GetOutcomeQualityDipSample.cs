using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetOutcomeQualityDipSample
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : IRequest<Result<DipSampleSummaryDto>>
    {
        public required Guid DipSampleId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<DipSampleSummaryDto>>
    {
        public async Task<Result<DipSampleSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

            var query =
                from sample in context.OutcomeQualityDipSamples
                join contract in context.Contracts on sample.ContractId equals contract.Id
                where sample.Id == request.DipSampleId
                select new DipSampleSummaryDto(contract.Description, sample.PeriodFrom);

            var dipSample = await query.FirstAsync(cancellationToken);

            return Result<DipSampleSummaryDto>.Success(dipSample);
        }
    }

    class Validator : AbstractValidator<Query>
    {
        readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(q => q.DipSampleId)
                    .Must(Exist)
                    .WithMessage("The requested dip sample was not found");
            });
        }

        bool Exist(Guid dipSampleId) => unitOfWork.DbContext.OutcomeQualityDipSamples.Any(d => d.Id == dipSampleId);
    }

}
