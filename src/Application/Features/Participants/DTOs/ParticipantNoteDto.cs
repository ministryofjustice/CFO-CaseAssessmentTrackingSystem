using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

[Description("Notes")]
public class ParticipantNoteDto
{
    [Description("Note Id")] public string Id { get; set; } = string.Empty;
    [Description("Message")] public string Message { get; set; } = string.Empty;
    [Description("Call Reference")] public string? CallReference { get; set; }
    [Description("CATS Identifier")] public string ParticipantId { get; set; } = string.Empty;
    [Description("Created By")] public string CreatedBy { get; set; } = string.Empty;
    [Description("Created Date")] public DateTime Created { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Note, ParticipantNoteDto>();
        }
    }

}
