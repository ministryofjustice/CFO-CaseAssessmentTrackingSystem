namespace Cfo.Cats.Application.Features.Identity.DTOs;

[Description("Roles")]
public class ApplicationRoleDto
{
    [Description("Id")]
    public required string Id { get; set; }

    [Description("Name")]
    public required string Name { get; set; }

    [Description("Normalized Name")]
    public string? NormalizedName { get; set; }

    [Description("Description")]
    public string? Description { get; set; }
}
