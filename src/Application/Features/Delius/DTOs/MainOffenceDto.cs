namespace Cfo.Cats.Application.Features.Delius.DTOs;

public class MainOffenceDto
{
    public required string OffenceDescription { get; init; }
    public DateOnly? OffenceDate { get; init; }
    public DisposalDto[] Disposals { get; init; } = [];
    public bool IsDeleted { get; init; }
}