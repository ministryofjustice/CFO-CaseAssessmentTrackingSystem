using Cfo.Cats.Domain.Labels;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Labels.DTOs;

public record LabelDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    
    [JsonIgnore] // cannot serialize a smart enum easily
    public required LabelScope Scope { get; init; }
    
    public AppColour Colour { get; init; }
    
    public AppVariant Variant { get; init; } 

    public AppIcon AppIcon { get; init; }

    /// <summary>
    /// The number of contracts this label is assigned to (and therefore visible for).
    /// </summary>
    public int ContractCount { get; init; }

    /// <summary>
    /// A comma separated list of the contract ids this label is assigned to.
    /// Populated by the data layer; use <see cref="ContractIds"/> to consume.
    /// </summary>
    public string? ContractIdsRaw { get; init; }

    [JsonIgnore]
    public IReadOnlyCollection<string> ContractIds =>
        string.IsNullOrWhiteSpace(ContractIdsRaw)
            ? []
            : ContractIdsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
