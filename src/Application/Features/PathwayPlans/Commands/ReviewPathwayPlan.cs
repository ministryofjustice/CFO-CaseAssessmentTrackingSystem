using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class ReviewPathwayPlan
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required Guid PathwayPlanId { get; init; }
        public required string ParticipantId { get; init; }
        public LocationDto? Location { get; set; }
        
        [Description(description: "Review Date")]
        public required DateTime? ReviewDate { get; set; }
        public string? Review { get; set; }
        
        [Description(description: "Review Reason")]
        public required PathwayPlanReviewReason ReviewReason { get; set; } = PathwayPlanReviewReason.Default;
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlan = await unitOfWork.DbContext.PathwayPlans.FirstOrDefaultAsync(x => x.Id == request.PathwayPlanId, cancellationToken);

            if (pathwayPlan is null)
            {
                throw new NotFoundException("Cannot find pathway plan", request.PathwayPlanId);
            }

            pathwayPlan.Review(request.PathwayPlanId, request.ParticipantId, request.Location!.Id,
                request.ReviewDate!.Value, request.Review, request.ReviewReason);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.PathwayPlanId)
                .NotNull()
                .WithMessage("You must provide a Pathway Plan");
            
            RuleFor(x => x.Review)
                .NotNull()
                .WithMessage("You must provide a review comment")
                .MaximumLength(ValidationConstants.NotesLength)
                .WithMessage($"Maximum length of a review comment is {ValidationConstants.NotesLength}")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Review"));
            
            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.PathwayPlanId)
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");
                
                RuleFor(c => c.ReviewDate)
                    .NotNull().WithMessage("Review date is required")
                    .MustAsync((command, reviewDate, token) => 
                        HaveOccurredOnOrAfterLastReview(command.ParticipantId, reviewDate, token))
                    .WithMessage("The review date cannot be earlier than the most recent review for this participant");
            });
            
            RuleFor(x => x.ReviewReason)
                .NotNull()
                .Must(r => r.IsValidSelection())
                .WithMessage("A review reason must be selected.");

            RuleFor(x => x.Location)
                .NotNull()
                .WithMessage("You must provide a location");
        }

        private async Task<bool> ParticipantMustNotBeArchived(Guid pathwayPlanId, CancellationToken cancellationToken)
        {
            var participantId = await (from pp in _unitOfWork.DbContext.PathwayPlans
                                       join p in _unitOfWork.DbContext.Participants on pp.ParticipantId equals p.Id
                                       where (pp.Id == pathwayPlanId
                                       && p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                                       select p.Id
                                       )
                            .AsNoTracking()
                            .FirstOrDefaultAsync(cancellationToken);

            return participantId != null;
        }
        
        private async Task<bool> HaveOccurredOnOrAfterLastReview(string participantId, DateTime? reviewDate, CancellationToken cancellationToken)
        {
            if (reviewDate is null)
            {
                return false;
            }

            var latestReviewDate = await _unitOfWork.DbContext.PathwayPlanReviews
                .Where(r => r.PathwayPlan.ParticipantId == participantId)
                .MaxAsync(r => (DateTime?)r.ReviewDate, cancellationToken);
            
            if (latestReviewDate is null)
            {
                return true;
            }
            
            return reviewDate.Value.Date >= latestReviewDate.Value.Date;
        }
    }
}