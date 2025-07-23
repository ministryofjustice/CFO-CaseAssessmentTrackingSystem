using Cfo.Cats.Application.Features.Identity.DTOs;

namespace Cfo.Cats.Application.Features.Tenants.DTOs;

public class TenantHierarchyDto
{
    public required string Id { get; set; }
    public string? Description { get; set; }
    public string ParentId { get; set; } = string.Empty;

    public int Depth { get; set; }

    public IEnumerable<TenantHierarchyDto> Children { get; set; } = [];
    public IEnumerable<UserSummaryDto> Users { get; set; } = [];
}