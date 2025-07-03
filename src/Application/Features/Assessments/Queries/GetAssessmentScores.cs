using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Assessments.Queries;

public static class GetAssessmentScores
{
    [RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
    public class Query : IRequest<Result<IEnumerable<ParticipantAssessmentDto>>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<IEnumerable<ParticipantAssessmentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;            
        }

        public async Task<Result<IEnumerable<ParticipantAssessmentDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = (from pa in _unitOfWork.DbContext.ParticipantAssessments
                         join l in _unitOfWork.DbContext.Locations on pa.LocationId equals l.Id
                         where pa.ParticipantId == request.ParticipantId
                         select new ParticipantAssessmentDto
                         {
                             ParticipantId = pa.ParticipantId,
                             CreatedDate = pa.Created!.Value,
                             Completed = pa.Completed,
                             LocationId = pa.LocationId,
                             LocationName = l.Name, // Directly access LocationName
                             PathwayScore = pa.Scores.Select(s => new PathwayScore(s.Pathway, s.Score)).ToArray()
                         })
                         .AsNoTracking();

            var result = await query.ToListAsync(cancellationToken);

            return Result<IEnumerable<ParticipantAssessmentDto>>.Success(result);
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
                    .MustAsync(ParticipantAssessmentExist)
                    .WithMessage("Participant/Assessment not found");
            });
        }

        private async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == participantId, cancellationToken);

        private async Task<bool> ParticipantAssessmentExist(string participantId, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.ParticipantAssessments.AnyAsync(e => e.ParticipantId == participantId, cancellationToken);
    }
}