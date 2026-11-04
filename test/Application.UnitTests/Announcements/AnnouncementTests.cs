using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Domain.Common.Exceptions;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementTests
{
    private static Announcement Create(AnnouncementAudience audience = AnnouncementAudience.AllStaff)
        => Announcement.Create("General", audience, "Original description",
            [new AnnouncementLink("https://example.com/guidance", "Guidance")], "creator");

    [Test]
    public void CreationRequiresApprovalAndDoesNotNotify()
    {
        var announcement = Create();
        announcement.CreatorId.ShouldBe("creator");
        announcement.Version.ShouldNotBe(Guid.Empty);
        announcement.PendingRevision.ShouldNotBeNull();
        announcement.PublishedRevision.ShouldBeNull();
        announcement.DomainEvents.ShouldBeEmpty();
        announcement.PendingRevision.Links.Single().DisplayName.ShouldBe("Guidance");
    }

    [Test]
    public void AnotherUserCanApproveAndPublish()
    {
        var announcement = Create();
        announcement.Approve(announcement.Version, "reviewer");
        announcement.PendingRevision.ShouldBeNull();
        announcement.PublishedRevision.ShouldNotBeNull();
        announcement.PublishedRevision.ApprovedBy.ShouldBe("reviewer");
        announcement.PublishedRevision.PublishedAt.ShouldNotBeNull();
        announcement.PublishedRevision.AutoApproved.ShouldBeFalse();
        announcement.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<AnnouncementPublishedDomainEvent>();
    }

    [Test]
    public void CreatorCannotApprove()
    {
        var announcement = Create();
        Should.Throw<BusinessRuleValidationException>(() => announcement.Approve(announcement.Version, "creator"));
        announcement.PublishedRevision.ShouldBeNull();
        announcement.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void LastEditorCannotApprove()
    {
        var announcement = Create();
        announcement.Edit(announcement.Version, "Release Note",
            AnnouncementAudience.AllStaff, "Edited", [], "editor");
        Should.Throw<BusinessRuleValidationException>(() => announcement.Approve(announcement.Version, "editor"));
        announcement.Approve(announcement.Version, "creator");
        announcement.PublishedRevision!.Description.ShouldBe("Edited");
        announcement.PublishedRevision.ApprovedBy.ShouldBe("creator");
        announcement.PublishedRevision.AutoApproved.ShouldBeFalse();
    }

    [Test]
    public void SupportCanExplicitlyPublishItsOwnAnnouncement()
    {
        var announcement = Create();
        announcement.AutoApprove(announcement.Version, "creator", true);
        announcement.PublishedRevision!.AutoApproved.ShouldBeTrue();
        announcement.PublishedRevision.ApprovedBy.ShouldBe("creator");
    }

    [Test]
    public void AutoApprovalIsDeniedWithoutSupportPermission()
    {
        var announcement = Create();
        Should.Throw<BusinessRuleValidationException>(() => announcement.AutoApprove(announcement.Version, "creator", false));
        announcement.PublishedRevision.ShouldBeNull();
        announcement.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void EditingRetainsPublishedVersionUntilApproval()
    {
        var announcement = Create();
        announcement.Approve(announcement.Version, "reviewer");
        announcement.ClearDomainEvents();
        var publishedId = announcement.PublishedRevisionId;

        announcement.Edit(announcement.Version, "Release Note",
            AnnouncementAudience.InternalStaff, "New description", [], "editor");

        announcement.PublishedRevisionId.ShouldBe(publishedId);
        announcement.PublishedRevision!.Description.ShouldBe("Original description");
        announcement.PublishedRevision.Audience.ShouldBe(AnnouncementAudience.AllStaff);
        announcement.PendingRevision!.Description.ShouldBe("New description");
        announcement.PendingRevision.Audience.ShouldBe(AnnouncementAudience.InternalStaff);
        announcement.PendingRevision.ApprovedBy.ShouldBeNull();
        announcement.DomainEvents.ShouldBeEmpty();

        announcement.Approve(announcement.Version, "reviewer");
        announcement.PublishedRevision!.Description.ShouldBe("New description");
        announcement.PendingRevision.ShouldBeNull();
        announcement.Revisions.Count.ShouldBe(2);
        announcement.DomainEvents.Count.ShouldBe(1);
    }

    [Test]
    public void EditingPendingContentInvalidatesTheReviewersSnapshot()
    {
        var announcement = Create();
        var reviewedVersion = announcement.Version;
        announcement.Edit(reviewedVersion, "General",
            AnnouncementAudience.AllStaff, "Replacement", [], "editor");
        Should.Throw<BusinessRuleValidationException>(() => announcement.Approve(reviewedVersion, "reviewer"));
        announcement.PublishedRevision.ShouldBeNull();
    }

    [Test]
    public void ConcurrentApprovalCannotRepublishTheSameRevision()
    {
        var announcement = Create();
        var version = announcement.Version;
        announcement.Approve(version, "reviewer");
        Should.Throw<BusinessRuleValidationException>(() => announcement.Approve(version, "another-reviewer"));
        Should.Throw<BusinessRuleValidationException>(() => announcement.Approve(announcement.Version, "another-reviewer"));
        announcement.DomainEvents.Count.ShouldBe(1);
    }

    [Test]
    public void DeletedAnnouncementsCannotBeEditedOrPublished()
    {
        var announcement = Create();
        announcement.Delete(announcement.Version, "manager");
        announcement.DeletedAt.ShouldNotBeNull();
        announcement.DeletedBy.ShouldBe("manager");
        announcement.Revisions.Count.ShouldBe(1);
        Should.Throw<BusinessRuleValidationException>(() => announcement.Approve(announcement.Version, "reviewer"));
        Should.Throw<BusinessRuleValidationException>(() => announcement.Edit(
            announcement.Version, "General", AnnouncementAudience.AllStaff, "Edited", [], "editor"));
        Should.Throw<BusinessRuleValidationException>(() => announcement.Delete(announcement.Version, "manager"));
    }

    [Test]
    public void StaleEditsAndDeletesAreRejected()
    {
        var announcement = Create();
        var version = announcement.Version;
        announcement.Edit(version, "General", AnnouncementAudience.AllStaff, "Edited", [], "editor");
        Should.Throw<BusinessRuleValidationException>(() => announcement.Delete(version, "manager"));
        Should.Throw<BusinessRuleValidationException>(() => announcement.Edit(
            version, "General", AnnouncementAudience.AllStaff, "Stale", [], "editor"));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void EmptyDescriptionIsRejected(string description)
        => Should.Throw<BusinessRuleValidationException>(() => Announcement.Create(
            "General", AnnouncementAudience.AllStaff, description, [], "creator"));

    [Test]
    public void DescriptionLimitIsEnforced()
    {
        var description = new string('a', AnnouncementConstants.DescriptionMaximumLength);
        Announcement.Create("General", AnnouncementAudience.AllStaff, description, [], "creator")
            .PendingRevision!.Description.Length.ShouldBe(AnnouncementConstants.DescriptionMaximumLength);
        Should.Throw<BusinessRuleValidationException>(() => Announcement.Create(
            "General", AnnouncementAudience.AllStaff, description + "a", [], "creator"));
    }

    [Test]
    public void InvalidTypesAndAudiencesAreRejected()
    {
        Should.Throw<BusinessRuleValidationException>(() => Announcement.Create(
            "", AnnouncementAudience.AllStaff, "Text", [], "creator"));
        Should.Throw<BusinessRuleValidationException>(() => Announcement.Create(
            "General", (AnnouncementAudience)999, "Text", [], "creator"));
    }

    [TestCase("General")]
    [TestCase("Release Note")]
    [TestCase("Custom type")]
    public void RevisionsStoreTheirSelectedType(string typeName)
        => Announcement.Create(typeName, AnnouncementAudience.AllStaff, "Text", [], "creator")
            .PendingRevision!.TypeName.ShouldBe(typeName);

    [Test]
    public void TitleAndDisplayedAuthorAreRetainedPerRevision()
    {
        var announcement = Announcement.Create("General", AnnouncementAudience.AllStaff,
            "Description", [], "creator", "Custom title", "CFO Team", "Actual Creator");
        announcement.Approve(announcement.Version, "reviewer");
        announcement.Edit(announcement.Version, "Release Note", AnnouncementAudience.AllStaff,
            "Edited description", [], "editor", "Release 2", "Release Team", "Actual Editor");
        announcement.PublishedRevision!.Title.ShouldBe("Custom title");
        announcement.PublishedRevision.PostedBy.ShouldBe("CFO Team");
        announcement.PendingRevision!.Title.ShouldBe("Release 2");
        announcement.PendingRevision.PostedBy.ShouldBe("Release Team");
        announcement.PendingRevision.SubmittedByName.ShouldBe("Actual Editor");
        announcement.Approve(announcement.Version, "second-reviewer");
        announcement.Revisions.First().ApprovedBy.ShouldBe("reviewer");
        announcement.Revisions.Last().ApprovedBy.ShouldBe("second-reviewer");
    }

    [TestCase("", "Author")]
    [TestCase("Title", "")]
    public void EmptyCustomTitlesAndAuthorsAreRejected(string title, string author)
        => Should.Throw<BusinessRuleValidationException>(() => Announcement.Create(
            "General", AnnouncementAudience.AllStaff, "Text", [], "creator", title, author, "Account name"));

    [Test]
    public void CustomTitlesAndAuthorsRespectLengthLimits()
    {
        Should.Throw<BusinessRuleValidationException>(() => Announcement.Create(
            "General", AnnouncementAudience.AllStaff, "Text", [], "creator",
            new string('a', AnnouncementConstants.TitleMaximumLength + 1), "Name", "Name"));
        Should.Throw<BusinessRuleValidationException>(() => Announcement.Create(
            "General", AnnouncementAudience.AllStaff, "Text", [], "creator",
            "Title", new string('a', AnnouncementConstants.PostedByMaximumLength + 1), "Name"));
    }
}
