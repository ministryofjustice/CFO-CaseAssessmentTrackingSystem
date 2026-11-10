using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.HmppsRecords;

public partial class ProbationRecord
{
    [Parameter]
    [EditorRequired]
    public IHmppsExternalApiService Service { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string Crn { get; set; } = default!;

    private Result<PersonResponsibleOfficerDto>? _officer;
    private Result<PersonLicencesDto>? _licences;
    private Result<OffenceDto[]>? _offences;
    private Result<SentenceDto[]>? _sentences;

    protected override async Task OnInitializedAsync()
    {
        var officer = Service.GetPersonResponsibleOfficerAsync(Crn, CancellationToken.None);
        var licences = Service.GetLicenceConditionsAsync(Crn, CancellationToken.None);
        var offences = Service.GetOffencesAsync(Crn, CancellationToken.None);
        var sentences = Service.GetSentencesAsync(Crn, CancellationToken.None);

        await Task.WhenAll(officer, licences, offences, sentences);

        _officer = officer.Result;
        _licences = licences.Result;
        _offences = offences.Result.FromSystem(HmppsSystemSource.ProbationSystems);
        _sentences = sentences.Result.FromSystem(HmppsSystemSource.ProbationSystems);
    }

    private static List<LicenceDto> OrderLicences(IEnumerable<LicenceDto>? licences) =>
        (licences ?? [])
            .OrderByDescending(l => l.Status == "ACTIVE")
            .ThenByDescending(l => HmppsDisplay.IsCurrentLicence(l.Status))
            .ThenByDescending(l => l.UpdatedDate ?? l.ApprovedDate ?? l.CreatedDate)
            .ToList();
}
