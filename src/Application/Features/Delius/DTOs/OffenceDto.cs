namespace Cfo.Cats.Application.Features.Delius.DTOs;

public class OffenceDto
{
    public required string Crn { get; init; }
    public MainOffenceDto[] MainOffences { get; init; } = [];
    public bool IsDeleted { get; init; }
}