
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class PriCodeDto
{
    [Description("CATS Participant Id")]
    public string? ParticipantId { get; set; }

    [Description("PRI Code")]
    public int? Code { get; set; }

    [Description("Community Support Worker")]
    public string? CommunitySupportWorker { get;}

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PriCode, PriCodeDto>(MemberList.None)
                .ForMember(x => x.CommunitySupportWorker, s => s.MapFrom(y => y.CreatedBy));
        }
    }

}
