using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Queries;

public static class GetPathwayPlanReviewHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : ParticipantDetailsQuery<PathwayPlanReviewHistoryDto[]>;

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PathwayPlanReviewHistoryDto[]>>
    {
        public async Task<Result<PathwayPlanReviewHistoryDto[]>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var baseQuery =
                from review in unitOfWork.DbContext.PathwayPlanReviews.AsNoTracking()
             
                join participant in unitOfWork.DbContext.Participants
                    on review.PathwayPlan.ParticipantId equals participant.Id
                join location in unitOfWork.DbContext.Locations
                    on review.LocationId equals location.Id
                join user in unitOfWork.DbContext.Users
                    on review.CreatedBy equals user.Id
                    
                where review.PathwayPlan.ParticipantId == request.ParticipantId
                orderby review.ReviewDate descending
                select new
                {
                    review.Id,
                    review.ParticipantId,
                    review.ReviewDate,
                    review.ReviewReason,
                    review.Review,
                    review.Created,
                    review.CreatedBy,

                    UserDisplayName = user.DisplayName,

                    LocationId = location.Id,
                    LocationName = location.Name,

                    EnrolmentStatus = participant.EnrolmentStatus
                };
            
            var queryResultList = await baseQuery
                .ToListAsync(cancellationToken);
            
            var aWeekAgo = DateTime.UtcNow.Date.AddDays(-7);
            var count = queryResultList.Count;
            
            var result = queryResultList
                .Select((x, index) =>
                {
                    int? daysSinceLastReview = null;

                    if (index < count - 1)
                    {
                        var previousReviewDate = queryResultList[index + 1].ReviewDate.Date;
                        var currentReviewDate = x.ReviewDate.Date;

                        daysSinceLastReview =
                            (currentReviewDate - previousReviewDate).Days;
                    }

                    var isActive = x.EnrolmentStatus?.ParticipantIsActive() == true;

                    var canEdit = x.Created >= aWeekAgo;

                    var correctUser = x.CreatedBy == request.CurrentUser.UserId;

                    return new PathwayPlanReviewHistoryDto
                    {
                        Id = x.Id,
                        ParticipantId = x.ParticipantId,
                        ReviewDate = x.ReviewDate,
                        ReviewedBy = x.UserDisplayName,
                        LocationId = x.LocationId,
                        LocationName = x.LocationName,
                        ReviewReason = x.ReviewReason,
                        Review = x.Review,
                        Created = x.Created!.Value,
                        DaysSinceLastReview = daysSinceLastReview,
                        IsEditable = isActive && canEdit && correctUser
                    };
                })
                .ToArray();

            return Result<PathwayPlanReviewHistoryDto[]>.Success(result);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .NotEmpty()
                .Length(9)
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