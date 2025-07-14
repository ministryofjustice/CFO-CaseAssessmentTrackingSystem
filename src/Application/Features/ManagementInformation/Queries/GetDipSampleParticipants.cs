using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetDipSampleParticipants 
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : IRequest<Result<IEnumerable<DipSampleParticipantSummaryDto>>>
    {
        public required Guid DipSampleId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<IEnumerable<DipSampleParticipantSummaryDto>>>
    {
        public async Task<Result<IEnumerable<DipSampleParticipantSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var query =
                from sample in context.DipSampleParticipants
                join participant in context.Participants on sample.ParticipantId equals participant.Id
                join reviewer in context.Users on sample.ReviewedBy equals reviewer.Id
                where sample.DipSampleId == request.DipSampleId
                select new DipSampleParticipantSummaryDto(
                    sample.ParticipantId, 
                    participant.FullName!, 
                    participant.Owner.DisplayName,
                    sample.LocationType, 
                    participant.EnrolmentLocation.Name,
                    sample.IsCompliant, 
                    sample.ReviewedOn,
                    reviewer.DisplayName);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var participants = await query
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if(participants is not { Count: > 0 })
            {
                Result<IEnumerable<DipSampleParticipantSummaryDto>>.Failure("No participants found to sample.");
            }

            return Result<IEnumerable<DipSampleParticipantSummaryDto>>.Success(participants);
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

        bool Exist(Guid dipSampleId) => unitOfWork.DbContext.DipSamples.Any(d => d.Id == dipSampleId);
    }
}
