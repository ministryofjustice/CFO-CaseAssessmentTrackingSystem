namespace Cfo.Cats.Application.Features.Initiatives.DTOs;

public record InitiativeDto
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }
    public required string ContractId { get; init; }
    public required string Contract { get; init; }
    public DateTime LifetimeStart { get; init; }
    public DateTime LifetimeEnd { get; init; }
}
