using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class IdentityAuditTrailDto
{

    [Description("User Name")]
    public string? UserName { get; set; }

    [Description("Performed By")]
    public string? PerformedBy { get; set; }
        
    [Description("Remote IP Address")]
    public string? IpAddress { get; set; }

    [Description("Date Time")]
    public DateTime DateTime { get; set; }

    [Description("Action Type")]
    public IdentityActionType? ActionType { get;set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<IdentityAuditTrail, IdentityAuditTrailDto>();
        }
    }

}