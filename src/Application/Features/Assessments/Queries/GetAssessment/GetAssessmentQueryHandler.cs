using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Application.Features.Assessments.Queries.GetAssessment;

public class GetAssessmentQueryHandler : IRequestHandler<GetAssessmentQuery, Result<AssessmentDto>>
{

    public async Task<Result<AssessmentDto>> Handle(GetAssessmentQuery request, CancellationToken cancellationToken)
    {
        AssessmentDto dto = new AssessmentDto();
        return await Result<AssessmentDto>.SuccessAsync(dto);
    }
}

