using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cfo.Cats.Application.Features.PRIs.Commands.AddPRI;

namespace Cfo.Cats.Application.Features.PRIs.DTOs;

public class PRIPrintVersionDto
{
    public PRIDto? _priDto { get; set; }
    [Description("Custody Support Worker")]    public string CreatedBy { get; set; } = string.Empty;
    [Description("Participant Name")] public string ParticipantFullName { get; set; } = string.Empty;
    [Description("PRI Created On")] public DateTime PriCreatedOn { get; set; }
    public PathwayPlanDto? _pathwayPlanDto { get; set; }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Domain.Entities.PRIs.PRI, PRIPrintVersionDto>(MemberList.None)
             .ForMember(target => target.CreatedBy,
                options => options.MapFrom(source => source.CreatedBy))
             .ForMember(target => target.ParticipantFullName,
                options => options.MapFrom(source => source.Participant!.FullName))
             .ForMember(target => target.PriCreatedOn,
                options => options.MapFrom(source => source.Created));
        }
    }
}
