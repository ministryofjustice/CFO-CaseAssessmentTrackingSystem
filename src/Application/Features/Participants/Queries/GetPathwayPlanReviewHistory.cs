using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetPathwayPlanReviewHistoryHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : ParticipantDetailsQuery<PathwayPlanReviewHistoryDto[]>;
    
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PathwayPlanReviewHistoryDto[]>>
    {
        public async Task<Result<PathwayPlanReviewHistoryDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var baseQuery = 
                (from pprh in unitOfWork.DbContext.PathwayPlanReviews
                    join l in unitOfWork.DbContext.Locations on pprh.LocationId equals l.Id
                    join createdBy in unitOfWork.DbContext.Users on pprh.CreatedBy equals createdBy.Id
                     where pprh.ParticipantId == request.ParticipantId
                    select new PathwayPlanReviewHistoryDto
                    {
                        Id = pprh.Id,
                        ParticipantId = pprh.ParticipantId,
                        ReviewDate = pprh.Created!.Value,
                        ReviewedBy = createdBy.DisplayName!,
                        LocationId = pprh.LocationId,
                        LocationName = l.Name,
                        ReviewReason = pprh.ReviewReason,
                        Review = pprh.Review
                    })
                .AsNoTracking()
                .OrderByDescending(r => r.ReviewDate);
                    
            var queryResultList = await baseQuery.ToListAsync(cancellationToken);

            var result = queryResultList.Select((item, index) => {
                var daysDifference = 0;
                var nextItem = index < queryResultList.Count - 1 ? queryResultList[index + 1] : null;

                if (nextItem != null)
                {
                    var laterDate = DateOnly.FromDateTime(item.ReviewDate);
                    var earlierDate = DateOnly.FromDateTime(nextItem.ReviewDate);
                    daysDifference = (laterDate.ToDateTime(TimeOnly.MinValue) - earlierDate.ToDateTime(TimeOnly.MinValue)).Days;
                }

                return new PathwayPlanReviewHistoryDto
                {
                    Id = item.Id,
                    ParticipantId = item.ParticipantId,
                    ReviewDate = item.ReviewDate,
                    ReviewedBy = item.ReviewedBy,
                    LocationId = item.LocationId,
                    LocationName = item.LocationName,
                    ReviewReason = item.ReviewReason,
                    Review = item.Review,
                    DaysSinceLastReview = daysDifference
                };
            }).ToArray();

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
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
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