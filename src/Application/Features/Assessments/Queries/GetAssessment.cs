using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.Caching;
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
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Query : IRequest<Result<Assessment>>
    {
        public required string ParticipantId { get; set; }
        public Guid? AssessmentId { get; set; }

    }

    internal class Handler : IRequestHandler<Query, Result<Assessment>>
    {
        private readonly IApplicationDbContext _context;
        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Assessment>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _context.ParticipantAssessments
                .Where(p => p.ParticipantId == request.ParticipantId);

            if (request.AssessmentId is not null)
            {
                query = query.Where(p => p.Id == request.AssessmentId);
            }

            var pa = await query.OrderByDescending(pa => pa.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (pa is null)
            {
                return Result<Assessment>.Failure(["Participant not found"]);
            }

            Assessment assessment = JsonConvert.DeserializeObject<Assessment>(pa.AssessmentJson,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;
            return await Result<Assessment>.SuccessAsync(assessment);
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
                .MaximumLength(9);
        }
    }
}