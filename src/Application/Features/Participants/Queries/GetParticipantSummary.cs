using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantSummary
{
    [RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
    public class Query : IAuditableRequest<Result<ParticipantSummaryDto>>
    {
        public required string ParticipantId { get; set; } 
        public required UserProfile CurrentUser { get; set; }

        public string Identifier() => ParticipantId;
    }

    public class Handler(IUnitOfWork unitOfWork, IRightToWorkSettings rtwSettings) : IRequestHandler<Query, Result<ParticipantSummaryDto>>
    {
        public async Task<Result<ParticipantSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
            var participantId = request.ParticipantId;

            var activeStatuses = EnrolmentStatus.ActiveList;

#pragma warning disable CS8601, CS8602, CS8629
            var query = from p in db.Participants
                        where p.Id == participantId
                        select new ParticipantSummaryDto
                        {
                            Id = p.Id,
                            TenantName = p.Owner.TenantName,
                            ParticipantName = $"{p.FirstName} {p.LastName}",
                            OwnerName = p.Owner.DisplayName,
                            LocationType = p.CurrentLocation.LocationType,
                            Location = p.CurrentLocation.Name,
                            EnrolmentLocation = p.EnrolmentLocation.Name,
                            EnrolmentLocationJustification = p.EnrolmentLocationJustification,
                            DateOfBirth = p.DateOfBirth,
                            RiskDue = p.RiskDue,
                            Nationality = p.Nationality,
                            BioDue = p.BioDue,
                            IsActive = EnrolmentStatus.ActiveList.Contains(p.EnrolmentStatus),
                            ActiveInFeed = p.ActiveInFeed,
                            LastSync = p.LastSyncDate ?? p.Created!.Value,
                            DateOfFirstConsent = p.DateOfFirstConsent,
                            EnrolmentStatus = p.EnrolmentStatus,
                            ConsentStatus = p.ConsentStatus,
                            RiskDueReason = p.RiskDueReason,
                            HasActiveRightToWork =
                                (from rtw in p.RightToWorks
                                 where rtw.Lifetime.EndDate >= DateTime.UtcNow.Date
                                 select rtw).Any(),
                            Assessments =
                                (from assessment in db.ParticipantAssessments
                                 where assessment.ParticipantId == participantId
                                 select new AssessmentSummaryDto
                                 {
                                     AssessmentId = assessment.Id,
                                     AssessmentDate = assessment.Created,
                                     AssessmentCreator = assessment.CreatedBy,
                                     Completed = assessment.Completed,
                                     AssessmentScored = assessment.Scores.All(s => s.Score >= 0)
                                 }).ToArray(),
                            LatestRisk =
                                (from risk in db.Risks
                                 where risk.ParticipantId == participantId
                                 orderby risk.Created descending
                                 select new RiskSummaryDto
                                 {
                                     Created = risk.Created.Value,
                                     CreatedBy = risk.CreatedBy,
                                     ParticipantId = risk.ParticipantId,
                                     ReviewReason = risk.ReviewReason,
                                     ReferredOn = risk.ReferredOn,
                                     ReviewJustification = risk.ReviewJustification
                                 }).FirstOrDefault(),
                            PathwayPlan =
                                (from pathwayPlan in db.PathwayPlans
                                 where pathwayPlan.ParticipantId == participantId
                                 select new
                                 {
                                     Created = pathwayPlan.Created.Value,
                                     pathwayPlan.CreatedBy,
                                     LastReview = (from review in pathwayPlan.ReviewHistories
                                                   orderby review.Created
                                                   select new
                                                   {
                                                       ReviewedOn = review.Created,
                                                       ReviewedBy = review.CreatedBy
                                                   }).FirstOrDefault()
                                 }).Select(pathwayPlan => new PathwayPlanSummaryDto
                                 {
                                     Created = pathwayPlan.Created,
                                     CreatedBy = pathwayPlan.CreatedBy,
                                     LastReviewed = pathwayPlan.LastReview.ReviewedOn,
                                     LastReviewedBy = pathwayPlan.LastReview.ReviewedBy,
                                 }).FirstOrDefault(),
                            BioSummary =
                                (from bio in db.ParticipantBios
                                 where bio.ParticipantId == participantId
                                 select new BioSummaryDto
                                 {
                                     BioCreator = bio.CreatedBy,
                                     BioDate = bio.Created,
                                     BioId = bio.Id,
                                     BioStatus = bio.Status
                                 }).FirstOrDefault(),
                            LatestPri =
                                (from pri in db.PRIs
                                 where pri.ParticipantId == participantId
                                 orderby pri.Created descending
                                 select new PriSummaryDto
                                 {
                                     AbandonReason = pri.AbandonReason,
                                     ActualReleaseDate = pri.ActualReleaseDate,
                                     CompletedBy = pri.CompletedBy,
                                     CompletedOn = pri.CompletedOn,
                                     Created = pri.Created.Value,
                                     CreatedBy = pri.CreatedBy,
                                     ObjectiveId = pri.ObjectiveId,
                                     ObjectiveTasks = (from task in db.ObjectiveTasks
                                                       where task.ObjectiveId == pri.ObjectiveId
                                                       select new ObjectiveTaskDto
                                                       {
                                                           Created = task.Created.Value,
                                                           CreatedBy = task.CreatedBy,
                                                           Description = task.Description,
                                                           Due = task.Due,
                                                           Id = task.Id,
                                                           IsMandatory = task.IsMandatory,
                                                           Index = task.Index,
                                                           Completed = task.Completed,
                                                           ObjectiveId = task.ObjectiveId
                                                       }).ToArray(),
                                     ParticipantId = pri.ParticipantId,
                                     Status = pri.Status
                                 }).FirstOrDefault()
                        };
#pragma warning restore CS8601, CS8602, CS8629

            var summary = await query
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (summary == null)
            {
                throw new NotFoundException(nameof(ParticipantSummaryDto), request.ParticipantId);
            }

            summary.IsRightToWorkRequired = rtwSettings.NationalitiesExempted.Any(s => s.Equals(summary.Nationality!, StringComparison.OrdinalIgnoreCase)) == false;

            return Result<ParticipantSummaryDto>.Success(summary);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));        

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist");
            });
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}