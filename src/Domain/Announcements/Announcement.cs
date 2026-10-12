using Cfo.Cats.Domain.Announcements.Rules;
using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Announcements;

public class Announcement : BaseAuditableEntity<Guid>
{
    private readonly List<AnnouncementRevision> _revisions = [];

    private Announcement() { }

    public string CreatorId { get; private set; } = string.Empty;
    public Guid Version { get; private set; }
    public Guid? PendingRevisionId { get; private set; }
    public Guid? PublishedRevisionId { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    public IReadOnlyCollection<AnnouncementRevision> Revisions => _revisions.AsReadOnly();
    public AnnouncementRevision? PendingRevision => _revisions.SingleOrDefault(r => r.Id == PendingRevisionId);
    public AnnouncementRevision? PublishedRevision => _revisions.SingleOrDefault(r => r.Id == PublishedRevisionId);

    public static Announcement Create(
        string typeName,
        AnnouncementAudience audience,
        string description,
        IEnumerable<AnnouncementLink> links,
        string creatorId,
        string? title = null,
        string? postedBy = null,
        string? submittedByName = null)
    {
        var announcement = new Announcement
        {
            Id = Guid.CreateVersion7(),
            CreatorId = creatorId
        };
        announcement.Submit(typeName, audience, description, links, creatorId,
            title, postedBy ?? creatorId, submittedByName ?? creatorId);
        return announcement;
    }

    public void Edit(
        Guid expectedVersion,
        string typeName,
        AnnouncementAudience audience,
        string description,
        IEnumerable<AnnouncementLink> links,
        string submittedBy,
        string? title = null,
        string? postedBy = null,
        string? submittedByName = null)
    {
        CheckAvailable(expectedVersion);
        Submit(typeName, audience, description, links, submittedBy,
            title, postedBy ?? submittedBy, submittedByName ?? submittedBy);
    }

    public void Approve(Guid expectedVersion, string approvedBy)
    {
        CheckAvailable(expectedVersion);
        CheckRule(new AnnouncementWorkflowRule(
            approvedBy == PendingRevision?.SubmittedBy,
            "You cannot approve a revision you submitted."));
        Publish(approvedBy, false);
    }

    public void AutoApprove(Guid expectedVersion, string approvedBy, bool isSystemSupport)
    {
        CheckAvailable(expectedVersion);
        CheckRule(new AnnouncementWorkflowRule(!isSystemSupport, "Only System Support can approve and publish immediately."));
        Publish(approvedBy, true);
    }

    public void Delete(Guid expectedVersion, string deletedBy)
    {
        CheckAvailable(expectedVersion);
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        Version = Guid.CreateVersion7();
    }

    private void Submit(
        string typeName,
        AnnouncementAudience audience,
        string description,
        IEnumerable<AnnouncementLink> links,
        string submittedBy,
        string? title,
        string postedBy,
        string submittedByName)
    {
        CheckRule(new AnnouncementWorkflowRule(
            string.IsNullOrWhiteSpace(typeName) || typeName.Length > AnnouncementConstants.TypeNameMaximumLength,
            "Choose a valid type."));
        title ??= typeName;
        CheckRule(new AnnouncementWorkflowRule(
            string.IsNullOrWhiteSpace(title) || title.Length > AnnouncementConstants.TitleMaximumLength,
            $"Title is required and must not exceed {AnnouncementConstants.TitleMaximumLength} characters."));
        CheckRule(new AnnouncementWorkflowRule(
            string.IsNullOrWhiteSpace(postedBy) || postedBy.Length > AnnouncementConstants.PostedByMaximumLength,
            $"Posted by is required and must not exceed {AnnouncementConstants.PostedByMaximumLength} characters."));
        CheckRule(new AnnouncementWorkflowRule(!Enum.IsDefined(audience), "Choose a valid audience."));
        CheckRule(new AnnouncementWorkflowRule(
            string.IsNullOrWhiteSpace(description) || description.Length > AnnouncementConstants.DescriptionMaximumLength,
            $"Description is required and must not exceed {AnnouncementConstants.DescriptionMaximumLength} characters."));
        CheckRule(new AnnouncementWorkflowRule(string.IsNullOrWhiteSpace(submittedBy), "An author is required."));
        var revisionLinks = links.ToArray();
        foreach (var link in revisionLinks)
        {
            CheckRule(new AnnouncementWorkflowRule(
                string.IsNullOrWhiteSpace(link.Url) || link.Url.Length > AnnouncementConstants.UrlMaximumLength,
                $"Links are required and must not exceed {AnnouncementConstants.UrlMaximumLength} characters."));
            CheckRule(new AnnouncementWorkflowRule(
                link.DisplayName?.Length > AnnouncementConstants.DisplayNameMaximumLength,
                $"Link labels must not exceed {AnnouncementConstants.DisplayNameMaximumLength} characters."));
        }
        var revision = new AnnouncementRevision(typeName, audience, description.Trim(), revisionLinks,
            submittedBy, title.Trim(), postedBy.Trim(), submittedByName);
        _revisions.Add(revision);
        PendingRevisionId = revision.Id;
        Version = Guid.CreateVersion7();
    }

    private void Publish(string approvedBy, bool autoApproved)
    {
        CheckRule(new AnnouncementWorkflowRule(PendingRevision is null, "There is no announcement awaiting approval."));
        CheckRule(new AnnouncementWorkflowRule(string.IsNullOrWhiteSpace(approvedBy), "An approver is required."));
        var revision = PendingRevision!;
        revision.Publish(approvedBy, autoApproved);
        PublishedRevisionId = revision.Id;
        PendingRevisionId = null;
        Version = Guid.CreateVersion7();
        AddDomainEvent(new AnnouncementPublishedDomainEvent(Id, revision));
    }

    private void CheckAvailable(Guid expectedVersion)
    {
        CheckRule(new AnnouncementWorkflowRule(DeletedAt is not null, "This announcement has been deleted."));
        CheckRule(new AnnouncementWorkflowRule(
            Version != expectedVersion,
            "This announcement has changed. Refresh the page and try again."));
    }
}
