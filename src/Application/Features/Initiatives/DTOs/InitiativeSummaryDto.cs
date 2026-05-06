namespace Cfo.Cats.Application.Features.Initiatives.DTOs;

public record InitiativeSummaryDto
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }
}
