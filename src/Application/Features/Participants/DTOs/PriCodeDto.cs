
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class PriCodeDto
{
    [Description("CATS Participant Id")]
    public string? ParticipantId { get; set; }

    [Description("PRI Code")]
    public int? Code { get; set; }

}
