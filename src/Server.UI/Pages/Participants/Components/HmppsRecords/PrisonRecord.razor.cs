using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.HmppsRecords;

public partial class PrisonRecord
{
    [Parameter]
    [EditorRequired]
    public IHmppsExternalApiService Service { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public string NomisNumber { get; set; } = default!;

    private Result<SentenceKeyDatesDto>? _keyDates;
    private Result<PersonResponsibleOfficerDto>? _officer;
    private Result<OffenceDto[]>? _offences;
    private Result<SentenceDto[]>? _sentences;

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

    protected override async Task OnInitializedAsync()
    {
        var keyDates = Service.GetLatestSentenceKeyDatesAsync(NomisNumber, CancellationToken.None);
        var officer = Service.GetPersonResponsibleOfficerAsync(NomisNumber, CancellationToken.None);
        var offences = Service.GetOffencesAsync(NomisNumber, CancellationToken.None);
        var sentences = Service.GetSentencesAsync(NomisNumber, CancellationToken.None);

        await Task.WhenAll(keyDates, officer, offences, sentences);

        _keyDates = keyDates.Result;
        _officer = officer.Result;
        _offences = offences.Result.FromSystem(HmppsSystemSource.PrisonSystems);
        _sentences = sentences.Result.FromSystem(HmppsSystemSource.PrisonSystems);
    }

    /// <summary>
    /// The key dates that are set, in the order a practitioner typically reads them. Overrides take precedence.
    /// </summary>
    private static List<(string Label, DateOnly Date)> KeyDates(SentenceKeyDatesDto dates)
    {
        var candidates = new (string Label, DateOnly? Date)[]
        {
            ("Sentence start", dates.Sentence?.StartDate),
            (dates.Release?.ConfirmedDate is not null ? "Release date (confirmed)" : "Release date", dates.Release?.ConfirmedDate ?? dates.Release?.Date),
            ("Conditional release (CRD)", dates.ConditionalRelease?.EffectiveDate),
            ("Automatic release (ARD)", dates.AutomaticRelease?.EffectiveDate),
            ("HDC eligibility (HDCED)", dates.HomeDetentionCurfew?.EligibilityOverrideDate ?? dates.HomeDetentionCurfew?.EligibilityDate),
            ("HDC approved (HDCAD)", dates.HomeDetentionCurfew?.ActualDate),
            ("HDC end", dates.HomeDetentionCurfew?.EndDate),
            ("Parole eligibility (PED)", dates.ParoleEligibility?.EffectiveDate),
            ("Approved parole (APD)", dates.ActualParoleDate),
            ("Non-parole (NPD)", dates.NonParole?.EffectiveDate),
            ("Post recall release (PRRD)", dates.PostRecallRelease?.EffectiveDate),
            ("Tariff", dates.TariffDate),
            ("Early removal scheme eligibility", dates.EarlyRemovalSchemeEligibilityDate),
            ("ROTL eligibility", dates.ReleaseOnTemporaryLicenceDate),
            ("Early term (ETD)", dates.EarlyTerm?.EffectiveDate),
            ("Mid term (MTD)", dates.MidTerm?.EffectiveDate),
            ("Late term (LTD)", dates.LateTerm?.EffectiveDate),
            ("Licence expiry (LED)", dates.LicenceExpiry?.EffectiveDate),
            ("Sentence expiry (SED)", dates.Sentence?.ExpiryOverrideDate ?? dates.Sentence?.ExpiryDate),
            ("Top-up supervision start", dates.TopupSupervision?.StartDate),
            ("Top-up supervision expiry (TUSED)", dates.TopupSupervision?.ExpiryOverrideDate ?? dates.TopupSupervision?.ExpiryDate),
        };

        return candidates
            .Where(c => c.Date is not null)
            .Select(c => (c.Label, c.Date!.Value))
            .ToList();
    }
}
