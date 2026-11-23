using Cfo.Cats.Application.Features.Offloc.DTOs;
using Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceData;
using Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.DMS.Offloc;

public partial class OffLocDetails
{
    [Parameter, EditorRequired] 
    public string NomisNumber { get; set; } = null!;

    [Parameter]
    public string? RecordIdentifier { get; set; }

    [Parameter]
    public string? Justification { get; set; }

    // The audited query is only used by the advanced search; the participant page
    // keeps its original, unaudited behaviour.
    protected override IQuery<Result<SentenceDataDto>> CreateQuery()
        => string.IsNullOrWhiteSpace(Justification)
            ? new GetSentenceDataQuery { NomisNumber = NomisNumber }
            : new GetSentenceDataForAdvancedSearchQuery
            {
                NomisNumber = NomisNumber,
                RecordIdentifier = RecordIdentifier ?? string.Empty,
                Justification = Justification,
                CurrentUser = CurrentUser
            };
}