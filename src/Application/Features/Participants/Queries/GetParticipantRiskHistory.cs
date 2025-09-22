using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public class GetParticipantRiskHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<RiskHistoryDto[]>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<RiskHistoryDto[]>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RiskHistoryDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var baseQuery = (from r in _unitOfWork.DbContext.Risks
                             join l in _unitOfWork.DbContext.Locations on r.LocationId equals l.Id
                             where r.ParticipantId == request.ParticipantId
                             select new RiskHistoryDto
                             {
                                 Id = r.Id,
                                 ParticipantId = r.ParticipantId,
                                 CreatedDate = r.Created!.Value,
                                 CreatedBy = r.CreatedBy!,
                                 Completed = r.Completed,
                                 CompletedBy = r.CompletedBy,
                                 LocationId = r.LocationId,
                                 LocationName = l.Name,
                                 RiskReviewReason = r.ReviewReason
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
                    DaysSinceLastReview = daysDifference
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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(x => x.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant not found");

                RuleFor(x => x.ParticipantId)
                    .MustAsync(ParticipantRiskExist)
                    .WithMessage("Participant/Risk not found");
            });
        }

        private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId, cancellationToken);

        private async Task<bool> ParticipantRiskExist(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Risks.AnyAsync(e => e.ParticipantId == participantId, cancellationToken);
    }
}
