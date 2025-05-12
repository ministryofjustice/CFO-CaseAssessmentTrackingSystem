namespace Cfo.Cats.Application.Features.Delius.DTOs;

public class RequirementDto
{
    public required string CategoryDescription { get; init; } 
    public required string SubCategoryDescription { get; init; } 
    public required string TerminationDescription { get; init; } 
    public required string Length { get; init; } 
    public required string UnitDescription { get; init; } 
    public bool IsDeleted { get; init; }
}