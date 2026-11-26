using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Offloc.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceData;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
public class GetSentenceDataQuery : IQuery<Result<SentenceDataDto>>
{
    public string NomisNumber { get; set; } = string.Empty;
}

public class GetSentenceDataQueryHandler(IOfflocService offlocService)
    : IQueryHandler<GetSentenceDataQuery, Result<SentenceDataDto>>
{
    public async Task<Result<SentenceDataDto>> Handle(GetSentenceDataQuery request, CancellationToken cancellationToken)
    {
        var sentenceData = await offlocService.GetSentenceDataAsync(request.NomisNumber);

        return sentenceData is NotFoundResult<SentenceDataDto>
            ? Result<SentenceDataDto>.Failure("No details found")
            : sentenceData;
    }
}
