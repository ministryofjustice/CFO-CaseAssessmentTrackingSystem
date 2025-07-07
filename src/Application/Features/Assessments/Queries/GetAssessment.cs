using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Assessments.Queries;

public static class GetAssessment
{    
    /// <summary>
    /// Returns an assessment, either the one specified by the AssessmentId or
    /// the latest on if that is not specified
    /// </summary>
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<Assessment>>
    {
        public required string ParticipantId { get; set; }
        public Guid? AssessmentId { get; set; }
    }

    internal class Handler : IRequestHandler<Query, Result<Assessment>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<Assessment>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.DbContext.ParticipantAssessments
                .Where(p => p.ParticipantId == request.ParticipantId);

            if (request.AssessmentId is not null)
            {
                query = query.Where(p => p.Id == request.AssessmentId);
            }

            var pa = await query.OrderByDescending(pa => pa.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (pa is null)
            {
                return Result<Assessment>.Failure(["Assessment not found"]);
            }

            Assessment assessment = JsonConvert.DeserializeObject<Assessment>(pa.AssessmentJson,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;

            return Result<Assessment>.Success(assessment);
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