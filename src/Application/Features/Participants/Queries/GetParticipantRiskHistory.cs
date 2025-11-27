using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantRiskHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : ParticipantDetailsQuery<RiskHistoryDto[]>
    {

    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<RiskHistoryDto[]>>
    {

        public async Task<Result<RiskHistoryDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var baseQuery = (from r in unitOfWork.DbContext.Risks
                             join l in unitOfWork.DbContext.Locations on r.LocationId equals l.Id
                             join createdBy in unitOfWork.DbContext.Users on r.CreatedBy equals createdBy.Id
                             join u in unitOfWork.DbContext.Users on r.CompletedBy equals u.Id into completedBy
                             from cb in completedBy.DefaultIfEmpty()
                             where r.ParticipantId == request.ParticipantId
                             select new RiskHistoryDto
                             {
                                 Id = r.Id,
                                 ParticipantId = r.ParticipantId,
                                 CreatedDate = r.Created!.Value,
                                 CreatedBy = createdBy.DisplayName!,
                                 Completed = r.Completed,
                                 CompletedBy = cb.DisplayName,
                                 LocationId = r.LocationId,
                                 LocationName = l.Name,
                                 RiskReviewReason = r.ReviewReason,
                                 RiskJustification = r.ReviewJustification 
                             })
                             .AsNoTracking()
                             .OrderByDescending(r => r.CreatedDate);

            var queryResultList = await baseQuery.ToListAsync(cancellationToken);

            var result = queryResultList.Select((item, index) => {
                int daysDifference = 0;
                var nextItem = index < queryResultList.Count - 1 ? queryResultList[index + 1] : null;

                if (nextItem != null)
                {
                    var laterDate = DateOnly.FromDateTime(item.CreatedDate);
                    var earlierDate = DateOnly.FromDateTime(nextItem.CreatedDate);
                    daysDifference = (laterDate.ToDateTime(TimeOnly.MinValue) - earlierDate.ToDateTime(TimeOnly.MinValue)).Days;
                }

                return new RiskHistoryDto
                {
                    Id = item.Id,
                    ParticipantId = item.ParticipantId,
                    CreatedDate = item.CreatedDate,
                    CreatedBy = item.CreatedBy,
                    Completed = item.Completed,
                    CompletedBy = item.CompletedBy,
                    LocationId = item.LocationId,
                    LocationName = item.LocationName,
                    RiskReviewReason = item.RiskReviewReason,
                    DaysSinceLastReview = daysDifference,
                    RiskJustification = item.RiskJustification?
                        .Substring(0, Math.Min(ValidationConstants.NotesLength, item.RiskJustification.Length))
                };
            }).ToArray();

            return Result<RiskHistoryDto[]>.Success(result);
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
        }
    }
}