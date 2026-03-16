using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Models;

public record ParticipantCascadingDetails
{
    public required string Id { get; init; }
    public required string FullName { get; init; }
    public required bool IsActive { get; init; }
    public ConsentStatus? ConsentStatus { get; set; }
    public DateOnly? DateOfFirstConsent { get; set; }
}