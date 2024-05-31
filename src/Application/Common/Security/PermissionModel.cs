namespace Cfo.Cats.Application.Common.Security;

public class PermissionModel
{
    public string Description { get; set; } = "Permission Description";
    public string Group { get; set; } = "Permission";
    public string? ClaimType { get; set; }
    public string ClaimValue { get; set; } = "";
    public bool Assigned { get; set; }

    public int? RoleId { get; set; }
    public int? UserId { get; set; }
}
