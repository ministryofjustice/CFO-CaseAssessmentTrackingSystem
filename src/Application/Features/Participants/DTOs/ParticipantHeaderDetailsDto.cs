namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// Core header details about a participant.
/// </summary>
public record ParticipantHeaderDetailsDto
{
    /// <summary>
    /// The participants date of birth
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }
    
    /// <summary>
    /// The location the participant enrolled at.
    /// </summary>
    public required string EnrolmentLocation { get; init; }
    
    /// <summary>
    /// The nationality of the participant.
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
}
