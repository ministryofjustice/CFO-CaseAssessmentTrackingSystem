using Cfo.Cats.Domain.Announcements;

namespace Cfo.Cats.Application.Features.Announcements.DTOs;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public Guid Version { get; set; }
    public string CreatorId { get; set; } = string.Empty;
    public AnnouncementRevisionDto? Published { get; set; }
    public AnnouncementRevisionDto? Pending { get; set; }
    public AnnouncementRevisionDto[] History { get; set; } = [];
}

public class AnnouncementRevisionDto
{
    public string TypeName { get; set; } = string.Empty;
    public AnnouncementAudience Audience { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PostedBy { get; set; } = string.Empty;
    public string SubmittedByName { get; set; } = string.Empty;
    public bool PostedByOverridden => !string.Equals(PostedBy, SubmittedByName, StringComparison.Ordinal);
    public string Description { get; set; } = string.Empty;
    public List<AnnouncementLinkDto> Links { get; set; } = [];
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool AutoApproved { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AnnouncementRevision, AnnouncementRevisionDto>();
            CreateMap<AnnouncementLink, AnnouncementLinkDto>();
        }
    }
}

public class AnnouncementLinkDto
{
    public string Url { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}
