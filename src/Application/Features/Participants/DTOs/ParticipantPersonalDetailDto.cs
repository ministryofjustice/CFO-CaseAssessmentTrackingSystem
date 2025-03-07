namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantPersonalDetailDto
{
    [Description("Preferred Name(s)")]
    public string? PreferredNames { get; set; }

    [Description("Preferred Pronouns")]
    public string? PreferredPronouns { get; set; }
}
