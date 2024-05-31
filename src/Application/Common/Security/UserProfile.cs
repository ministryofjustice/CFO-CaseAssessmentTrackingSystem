namespace Cfo.Cats.Application.Common.Security;

public class UserProfile
{
    public string? Provider { get; set; }
    public string? SuperiorName { get; set; }
    public int? SuperiorId { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public string? DisplayName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DefaultRole { get; set; }
    public string[]? AssignedRoles { get; set; }
    public int UserId { get; set; }
    public bool IsActive { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
}