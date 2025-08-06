using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetOutcomeQualityDipSampleParticipants 
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
    public class Query : PaginationFilter, IRequest<Result<PaginatedData<DipSampleParticipantSummaryDto>>>
    {
        public required Guid DipSampleId { get; set; }
        public bool OnlyShowInProgress { get; set; } = true;
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PaginatedData<DipSampleParticipantSummaryDto>>>
    {
        public async Task<Result<PaginatedData<DipSampleParticipantSummaryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var context = unitOfWork.DbContext;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var query =
                from sample in context.OutcomeQualityDipSampleParticipants
                join participant in context.Participants on sample.ParticipantId equals participant.Id
                join enrolmentLocation in context.Locations on participant.EnrolmentLocation.Id equals enrolmentLocation.Id
                join currentLocation in context.Locations on participant.CurrentLocation.Id equals currentLocation.Id
                join owner in context.Users on participant.OwnerId equals owner.Id
                join reviewer in context.Users on sample.CsoReviewedBy equals reviewer.Id into reviewers
                from reviewer in reviewers.DefaultIfEmpty()
                where sample.DipSampleId == request.DipSampleId
                where request.OnlyShowInProgress == false 
                    || sample.CsoReviewedOn == null
                where string.IsNullOrWhiteSpace(request.Keyword)
                    || sample.LocationType.Contains(request.Keyword)
                    || participant.LastName.Contains(request.Keyword)
                    || participant.Id.Contains(request.Keyword)
                    || enrolmentLocation.Name.Contains(request.Keyword)
                    || currentLocation.Name.Contains(request.Keyword)
                select new
                {
                    sample.ParticipantId,
                    ParticipantFullName = participant.FirstName + " " + participant.LastName,
                    ParticipantOwner = owner.DisplayName,
                    sample.LocationType,
                    CurrentLocationName = currentLocation.Name,
                    EnrolmentLocationName = enrolmentLocation.Name,
                    sample.CsoIsCompliant,
                    sample.CpmIsCompliant,
                    sample.CsoReviewedOn,
                    ReviewedBy = reviewer.DisplayName
                };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var count = await query
                .CountAsync(cancellationToken);

            var participants = await query
                .AsQueryable()
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .Select(dsp => new DipSampleParticipantSummaryDto(
                    dsp.ParticipantId,
                    dsp.ParticipantFullName,
                    dsp.ParticipantOwner,
                    dsp.LocationType,
                    dsp.CurrentLocationName,
                    dsp.EnrolmentLocationName,
                    dsp.CsoIsCompliant,
                    dsp.CpmIsCompliant,
                    dsp.CsoReviewedOn,
                    dsp.ReviewedBy))
                .ToListAsync(cancellationToken);

            if(participants is not { Count: > 0 })
            {
                Result<PaginatedData<DipSampleParticipantSummaryDto>>.Failure("No participants found to sample.");
            }

            return Result<PaginatedData<DipSampleParticipantSummaryDto>>.Success(
                new PaginatedData<DipSampleParticipantSummaryDto>(
                    participants,
                    count, 
                    request.PageNumber, 
                    request.PageSize));
        }
    }

    private class Validator : AbstractValidator<Query>
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
