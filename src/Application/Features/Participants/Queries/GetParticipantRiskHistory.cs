using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public class GetParticipantRiskHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<IEnumerable<RiskHistoryDto>>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<IEnumerable<RiskHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<RiskHistoryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = (from r in _unitOfWork.DbContext.Risks
                         join l in _unitOfWork.DbContext.Locations on r.LocationId equals l.Id
                         where r.ParticipantId == request.ParticipantId
                         select new RiskHistoryDto
                         {
                             Id = r.Id,
                             ParticipantId = r.ParticipantId,
                             CreatedDate = r.Created!.Value,
                             Completed = r.Completed,
                             CompletedBy = r.CompletedBy,
                             LocationId = r.LocationId,
                             LocationName = l.Name,
                             RiskReviewReason = r.ReviewReason
                         })
                         .AsNoTracking();

            var result = await query.ToListAsync(cancellationToken);

            return Result<IEnumerable<RiskHistoryDto>>.Success(result);
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
