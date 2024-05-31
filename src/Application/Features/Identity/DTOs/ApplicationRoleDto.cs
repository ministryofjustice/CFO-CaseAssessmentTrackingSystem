namespace Cfo.Cats.Application.Features.Identity.DTOs;

[Description("Roles")]
public class ApplicationRoleDto
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("Name")]
    public string Name { get; set; } = string.Empty;

    [Description("Normalized Name")]
    public string? NormalizedName { get; set; }

    [Description("Description")]
    public string? Description { get; set; }
}
