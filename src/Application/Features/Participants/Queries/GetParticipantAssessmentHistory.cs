
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Bios;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Entities.PRIs;
using System;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantAssessmentHistory
{
    [RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
    public class Query : IAuditableRequest<Result<ParticipantSummaryDto>>
    {
        public required string ParticipantId { get; set; }
        public required UserProfile CurrentUser { get; set; }

        public string Identifier() => ParticipantId;

    }
    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<ParticipantSummaryDto>>
    {

        public async Task<Result<ParticipantSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = from c in unitOfWork.DbContext.Participants
                        where c.Id == request.ParticipantId
                        select c;

            var summary = await query.ProjectTo<ParticipantSummaryDto>(mapper.ConfigurationProvider)
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (summary == null)
            {
                throw new NotFoundException(nameof(ParticipantSummaryDto), request.ParticipantId);
            }

            summary.Assessments = await unitOfWork.DbContext.ParticipantAssessments
                .Where(pa => pa.ParticipantId == request.ParticipantId && pa.Completed.HasValue == true)
                .ProjectTo<AssessmentSummaryDto>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            
            return Result<ParticipantSummaryDto>.Success(summary);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "ParticipantId"));
        }
    }
}