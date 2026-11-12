using Humanizer;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// Core header details about a participant.
/// </summary>
public record ParticipantHeaderDetailsDto
{
    /// <summary>
    /// The participants date of birth. Null until consent has been granted.
    /// </summary>
    public required DateOnly? DateOfBirth { get; init; }

    /// <summary>
    /// Whether consent has been approved. PII is withheld until it has.
    /// </summary>
    public bool IsConsentGranted { get; init; }
    
    /// <summary>
    /// The location the participant enrolled at.
    /// </summary>
    public required string EnrolmentLocation { get; init; }

    public string? EnrolmentJustification { get; init; }

    public string? AssessmentJustification { get; init; }
    
    /// <summary>
    /// The nationality of the participant. Null until consent has been granted.
    /// </summary>
    public required string? Nationality { get; init; }
    
    /// <summary>
    /// The date of first consent, if the consent has been approved.
    /// </summary>
    public required DateOnly? DateOfFirstConsent { get; init; }
    
    /// <summary>
    /// The date the record was last updated via the DMS feed.
    /// </summary>
    public required DateTime LastSync { get; init; }

    public string GetConsentDateHelperText() => DateOfFirstConsent switch
    {
        null => "Consent has not been confirmed",
        _ when DateOfBirth is null => $"Consent granted {DateOfFirstConsent.Humanize()}",
        _ => $"Consent granted {DateOfFirstConsent.Humanize()}, participant was {DateOfBirth.Value.CalculateAge(DateOfFirstConsent.Value)} years old"
    };
}
