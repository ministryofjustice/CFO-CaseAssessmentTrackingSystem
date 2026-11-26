namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public record CurrentParticipantContractDto
{
    public required string ParticipantId { get; init; }
    public string? OwnershipContractId { get; init; }
    public string? LocationContractId { get; init; }
    public string? LocationContractName { get; init; }
    public string? ContractId { get; set; }
    public string? ContractName { get; set; }
}
