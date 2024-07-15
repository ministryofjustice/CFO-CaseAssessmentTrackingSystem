using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Assessments.Queries;

public static class GetAssessmentScores
{
    [RequestAuthorize(Policy = PolicyNames.AllowCandidateSearch)]
    public class Query : IRequest<Result<IEnumerable<ParticipantAssessmentDto>>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<IEnumerable<ParticipantAssessmentDto>>>
    {
        
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<ParticipantAssessmentDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.DbContext.ParticipantAssessments
                .Include(pa => pa.Scores)
                .Where(pa => pa.ParticipantId == request.ParticipantId)
                .AsNoTracking()
                .ProjectTo<ParticipantAssessmentDto>(_mapper.ConfigurationProvider);

            var result = await query.ToListAsync(cancellationToken);
            return await Result<IEnumerable<ParticipantAssessmentDto>>.SuccessAsync(result);
        }
    }

}
