using Cfo.Cats.Application.Features.Offloc.DTOs;

namespace Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;

public class GetSentenceDataForAdvancedSearchQueryHandler(
    IOfflocService offlocService,
    IPrivilegedSearchAuditWriter privilegedSearchAuditWriter)
    : IQueryHandler<GetSentenceDataForAdvancedSearchQuery, Result<SentenceDataDto>>
{
    public async Task<Result<SentenceDataDto>> Handle(
        GetSentenceDataForAdvancedSearchQuery request,
        CancellationToken cancellationToken)
    {
        var nomisNumber = request.NomisNumber.Trim().ToUpperInvariant();
        var recordIdentifier = string.IsNullOrWhiteSpace(request.RecordIdentifier)
            ? nomisNumber
            : request.RecordIdentifier.Trim().ToUpperInvariant();
        var justification = string.IsNullOrWhiteSpace(request.Justification)
            ? "not-provided"
            : request.Justification.Trim();

        await privilegedSearchAuditWriter.RecordAdvancedSearchSentenceLookupAttemptAsync(
            request.CurrentUser,
            recordIdentifier,
            nomisNumber,
            justification,
            cancellationToken);

        var sentenceData = await offlocService.GetSentenceDataAsync(nomisNumber);
        if (sentenceData is NotFoundResult<SentenceDataDto>)
        {
            return Result<SentenceDataDto>.Failure("No details found");
        }

        return sentenceData;
    }
}
