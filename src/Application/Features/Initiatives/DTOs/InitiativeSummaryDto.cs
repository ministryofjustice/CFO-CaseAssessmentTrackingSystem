namespace Cfo.Cats.Application.Features.Initiatives.DTOs;

public record InitiativeSummaryDto
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }
    public DateOnly? InitiativeStartDate { get; init; }
    public DateOnly? InitiativeEndDate { get; init; }

    /// <summary>
    /// True when the initiative belongs to the same tenant/region as the case (participant) it's linked
    /// to — i.e. the initiative's contract tenant matches or is within the same hierarchy branch as the
    /// case owner's tenant. Used to determine whether unlinking this initiative from an objective requires
    /// CMPSM+ authorization and a justification.
    /// </summary>
    public bool IsWithinCaseTenant { get; init; } = true;
}
