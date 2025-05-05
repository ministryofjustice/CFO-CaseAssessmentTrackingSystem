namespace Cfo.Cats.Application.Features.Delius.DTOs;

public class DisposalDto
{
    public bool ShowDetails { get; set; } = false;
    public DateOnly? SentenceDate { get; init; }  
    public required string Length { get; init; }  
    public required string UnitDescription { get; init; } 
    public required string DisposalDetail { get; init; }
    public RequirementDto[] Requirements { get; init; } = [];
    public string? TerminationDescription { get; init; }
    public DateOnly? TerminationDate { get; init; }
    public bool IsDeleted { get; init; }

    public string SentenceLength => $"{Length} {UnitDescription}";
};