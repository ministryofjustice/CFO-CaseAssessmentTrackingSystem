using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class EditPathwayPlanReview
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public Guid ReviewId { get; init; }
        public int LocationId { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? Review { get; set; }
        public PathwayPlanReviewReason? ReviewReason { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pathwayPlanReview = await unitOfWork.DbContext.PathwayPlanReviews
                .Include(r => r.PathwayPlan)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);

            if (pathwayPlanReview is null)
            {
                throw new NotFoundException("Pathway Plan Review not found", request.ReviewId);
            }
            
            pathwayPlanReview.Update(
                request.LocationId,
                request.ReviewDate!.Value,
                request.Review,
                request.ReviewReason!);

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

            RuleFor(x => x.ReviewId)
                .NotNull()
            .WithMessage("You must provide a Pathway Plan Review");
            
            RuleFor(x => x.Review)
                .NotNull()
                .WithMessage("You must provide a review comment")
                .MaximumLength(ValidationConstants.NotesLength)
                .WithMessage($"Maximum length of a review comment is {ValidationConstants.NotesLength}")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Review"));
            
            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.ReviewId)
                    .MustAsync(ParticipantMustNotBeArchived)
                    .WithMessage("Participant is archived");

                RuleFor(c => c.ReviewDate)
                    .NotNull().WithMessage("Review date is required");
            });
            
            RuleFor(x => x.ReviewReason)
                .NotNull()
                .Must(r => r!.IsValidSelection())
                .WithMessage("A review reason must be selected.");
            
            RuleFor(x => x.LocationId)
                .NotNull()
                .WithMessage("You must provide a location");
        }

        private async Task<bool> ParticipantMustNotBeArchived(Guid pathwayPlanReviewId, CancellationToken cancellationToken)
        {
            var participantId = await _unitOfWork.DbContext.PathwayPlanReviews
                .Where(r => r.Id == pathwayPlanReviewId)
                .Select(r => r.PathwayPlan.ParticipantId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
            if (participantId == null)
            {return false;}
            
            return await _unitOfWork.DbContext.Participants
                .AsNoTracking()
                .AnyAsync(p => p.Id == participantId && 
                               p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value, cancellationToken);
        }
    }
}