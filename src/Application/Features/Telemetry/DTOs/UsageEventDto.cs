using Cfo.Cats.Domain.Entities.ManagementInformation;

namespace Cfo.Cats.Application.Features.Telemetry.DTOs;

[Description("Usage Telemetry")]
public class UsageEventDto
{
    [Description("Id")]
    public Guid Id { get; set; }

    [Description("Area")]
    public string Area { get; set; } = default!;

    [Description("Activity")]
    public string Activity { get; set; } = default!;

    [Description("User Id")]
    public string? UserId { get; set; }

    [Description("User")]
    public string? UserName { get; set; }

    [Description("Tenant")]
    public string? TenantId { get; set; }

    [Description("Context")]
    public string? Context { get; set; }

    [Description("Occurred On")]
    public DateTime OccurredOn { get; set; }

    private class Mapping : Profile
    {
        public Mapping() => CreateMap<UsageEvent, UsageEventDto>(MemberList.None);
    }
}
