using Cfo.Cats.Application.Features.Offloc.DTOs;
using Cfo.Cats.Domain.Entities.ManagementInformation;

namespace Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;

public class GetSentenceDataForAdvancedSearchQueryHandler(
    IOfflocService offlocService,
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetSentenceDataForAdvancedSearchQuery, Result<SentenceDataDto>>
{
    private const int UsageEventContextMaxLength = 256;

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

        var usageEvent = UsageEvent.Create(
            area: "Search",
            activity: "ParticipantAdvancedSearchSentenceLookup",
            userId: request.CurrentUser.UserId,
            userName: request.CurrentUser.UserName,
            tenantId: request.CurrentUser.TenantId,
            context: BuildAuditContext(recordIdentifier, nomisNumber, justification),
            occurredOn: DateTime.UtcNow);

        unitOfWork.DbContext.UsageEvents.Add(usageEvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var sentenceData = await offlocService.GetSentenceDataAsync(nomisNumber);
        if (sentenceData is NotFoundResult<SentenceDataDto>)
        {
            return Result<SentenceDataDto>.Failure("No details found");
        }

        return sentenceData;
    }

    private static string BuildAuditContext(string recordIdentifier, string nomisNumber, string justification)
    {
        var context = $"record={recordIdentifier} | nomis={nomisNumber} | justification={justification}";
        return context.Length <= UsageEventContextMaxLength
            ? context
            : context[..UsageEventContextMaxLength];
    }
}
