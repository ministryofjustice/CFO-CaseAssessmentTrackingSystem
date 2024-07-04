using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Identity.DTOs;

[Description("Notes")]
public class ApplicationUserNoteDto
{
    [Description("Note Id")] public string Id { get; set; } = string.Empty;
    [Description("Message")] public string Message { get; set; } = string.Empty;
    [Description("Call Reference")] public string? CallReference { get; set; }
    [Description("User Id")] public string ApplicationUserId { get; set; } = string.Empty;
    [Description("Created By")] public string CreatedBy { get; set; } = string.Empty;
    [Description("Created Date")] public DateTime Created { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Note, ApplicationUserNoteDto>(MemberList.None)
                .ReverseMap();
        }
    }
}
