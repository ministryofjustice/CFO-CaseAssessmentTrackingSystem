namespace Cfo.Cats.Domain.Announcements;

public class AnnouncementRevision
{
    private readonly List<AnnouncementLink> _links = [];

    private AnnouncementRevision() { }

    internal AnnouncementRevision(
        string typeName,
        AnnouncementAudience audience,
        string description,
        IEnumerable<AnnouncementLink> links,
        string submittedBy,
        string title,
        string postedBy,
        string submittedByName)
    {
        Id = Guid.CreateVersion7();
        TypeName = typeName;
        Audience = audience;
        Description = description;
        SubmittedBy = submittedBy;
        Title = title;
        PostedBy = postedBy;
        SubmittedByName = submittedByName;
        SubmittedAt = DateTime.UtcNow;
        _links.AddRange(links);
    }

    public Guid Id { get; private set; }
    public string TypeName { get; private set; } = string.Empty;
    public AnnouncementAudience Audience { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string PostedBy { get; private set; } = string.Empty;
    public string SubmittedByName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public IReadOnlyCollection<AnnouncementLink> Links => _links.AsReadOnly();
    public string SubmittedBy { get; private set; } = string.Empty;
    public DateTime SubmittedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public bool AutoApproved { get; private set; }

    internal void Publish(string approvedBy, bool autoApproved)
    {
        ApprovedBy = approvedBy;
        PublishedAt = DateTime.UtcNow;
        AutoApproved = autoApproved;
    }
}
