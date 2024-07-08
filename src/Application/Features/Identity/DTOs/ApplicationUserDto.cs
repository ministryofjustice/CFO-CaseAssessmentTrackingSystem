using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

[Description("Users")]
public class ApplicationUserDto
{
    [Description("User Id")] public string Id { get; set; } = string.Empty;

    [Description("User Name")] public string UserName { get; set; } = string.Empty;

    [Description("Display Name")] public string DisplayName { get; set; } = string.Empty;

    [Description("Provider")] public string? ProviderId { get; set; }

    [Description("Tenant Id")] public string? TenantId { get; set; }

    [Description("Tenant Name")] public string? TenantName { get; set; }

    [Description("Profile Photo")] public string? ProfilePictureDataUrl { get; set; }

    [Description("Email")] public string Email { get; set; } = string.Empty;

    [Description("Phone Number")] public string? PhoneNumber { get; set; }

    [Description("Superior Id")] public string? SuperiorId { get; set; }

    [Description("Superior Name")] public string? SuperiorName { get; set; }

    [Description("Assigned Roles")] public string[]? AssignedRoles { get; set; }

    [Description("Default Role")] public string? DefaultRole => AssignedRoles?.FirstOrDefault();

    [Description("Memorable Place")] public string MemorableDate { get; set; } = string.Empty;

    [Description("Memorable Place")] public string MemorablePlace { get; set; } = string.Empty;

    [Description("Is Active")] public bool IsActive { get; set; }

    [Description("Is Live")] public bool IsLive { get; set; }

    [Description("Password")] public string? Password { get; set; }

    [Description("Confirm Password")] public string? ConfirmPassword { get; set; }

    [Description("Status")] public DateTimeOffset? LockoutEnd { get; set; }

    [Description("Notes")] public List<ApplicationUserNoteDto> Notes { get; set; } = [];

    public UserProfile ToUserProfile()
    {
        return new UserProfile
        {
            UserId = Id,
            ProfilePictureDataUrl = ProfilePictureDataUrl,
            Email = Email,
            PhoneNumber = PhoneNumber,
            DisplayName = DisplayName,
            Provider = ProviderId,
            UserName = UserName,
            TenantId = TenantId,
            TenantName = TenantName,
            SuperiorId = SuperiorId,
            SuperiorName = SuperiorName,
            AssignedRoles = AssignedRoles,
            DefaultRole = DefaultRole
        };
    }

    public bool IsInRole(string role)
    {
        return AssignedRoles?.Contains(role) ?? false;
    }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>(MemberList.None)
                .ForMember(x => x.SuperiorName, s => s.MapFrom(y => y.Superior!.UserName))
                .ForMember(x => x.TenantName, s => s.MapFrom(y => y.Tenant!.Name))
                .ForMember(x => x.AssignedRoles, s => s.MapFrom(y => y.UserRoles.Select(r => r.Role.Name)))
            .ReverseMap()
                .ForMember(x => x.UserName, s => s.MapFrom(y => y.Email))
                .ForMember(x => x.Notes, s => s.Ignore())
                .ForMember(x => x.Tenant, s => s.Ignore())
            .AfterMap((dto, entity, context) =>
            {
                foreach(var noteDto in dto.Notes)
                {
                    var note = context.Mapper.Map<Note>(noteDto);
                    entity.AddNote(note);
                }
            });
        }
    }
}