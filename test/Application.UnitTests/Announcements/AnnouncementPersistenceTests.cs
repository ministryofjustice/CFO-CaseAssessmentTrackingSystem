using AutoMapper;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementPersistenceTests
{
    private static ApplicationDbContext Context()
        => new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=localhost;Database=announcement-model-tests;Integrated Security=true;TrustServerCertificate=true")
            .Options);

    [Test]
    public void ModelIncludesRevisionHistoryLinksAndOptimisticConcurrency()
    {
        using var context = Context();
        var model = context.Model.FindEntityType(typeof(Announcement))!;
        model.GetTableName().ShouldBe("Announcement");
        model.GetSchema().ShouldBe("Configuration");
        model.FindProperty(nameof(Announcement.Version))!.IsConcurrencyToken.ShouldBeTrue();
        model.FindProperty(nameof(Announcement.DeletedAt)).ShouldNotBeNull();
        model.FindNavigation(nameof(Announcement.Revisions))!.IsEagerLoaded.ShouldBeTrue();

        var revision = context.Model.FindEntityType(typeof(AnnouncementRevision))!;
        revision.GetTableName().ShouldBe("AnnouncementRevision");
        revision.FindProperty(nameof(AnnouncementRevision.Description))!.GetColumnType().ShouldBe("nvarchar(max)");
        revision.FindNavigation(nameof(AnnouncementRevision.Links))!.IsEagerLoaded.ShouldBeTrue();
    }

    [Test]
    public void EditsTrackANewRevisionWithoutChangingPublishedContent()
    {
        using var context = Context();
        var announcement = Announcement.Create("General",
            AnnouncementAudience.AllStaff, "Original", [new AnnouncementLink("https://example.com", "Link")], "creator");
        announcement.Approve(announcement.Version, "reviewer");
        context.Attach(announcement);
        announcement.Edit(announcement.Version, "Release Note",
            AnnouncementAudience.AllStaff, "Edited", [new AnnouncementLink("https://example.com/release", "Release")], "editor");
        context.ChangeTracker.DetectChanges();

        context.Entry(announcement).State.ShouldBe(EntityState.Modified);
        context.Entry(announcement).Property(x => x.Version).IsModified.ShouldBeTrue();
        context.Entry(announcement.PublishedRevision!).State.ShouldBe(EntityState.Unchanged);
        context.Entry(announcement.PendingRevision!).State.ShouldBe(EntityState.Added);
        context.Entry(announcement.PendingRevision!.Links.Single()).State.ShouldBe(EntityState.Added);
    }

    [Test]
    public void RevisionsMapToReadModelsIncludingLinksAndApproval()
    {
        var config = new MapperConfiguration(c => c.AddMaps(typeof(AnnouncementDto).Assembly));
        var mapper = config.CreateMapper();
        var announcement = Announcement.Create("Release Note",
            AnnouncementAudience.InternalStaff, "Release", [new AnnouncementLink("https://example.com", "Notes")], "creator");
        announcement.AutoApprove(announcement.Version, "creator", true);

        var dto = mapper.Map<AnnouncementRevisionDto>(announcement.PublishedRevision);

        dto.Title.ShouldBe("Release Note");
        dto.Description.ShouldBe("Release");
        dto.Audience.ShouldBe(AnnouncementAudience.InternalStaff);
        dto.Links.Single().DisplayName.ShouldBe("Notes");
        dto.ApprovedBy.ShouldBe("creator");
        dto.AutoApproved.ShouldBeTrue();
    }

    [TestCase("Actual Poster", false)]
    [TestCase("CFO Communications", true)]
    public void AuthorOverridesAreHighlightedWithoutReplacingTheRealAccount(string postedBy, bool overridden)
    {
        var config = new MapperConfiguration(c => c.AddMaps(typeof(AnnouncementDto).Assembly));
        var announcement = Announcement.Create("General",
            AnnouncementAudience.AllStaff, "Text", [], "actual-account-id", "Custom title", postedBy, "Actual Poster");
        var dto = config.CreateMapper().Map<AnnouncementRevisionDto>(announcement.PendingRevision);
        dto.Title.ShouldBe("Custom title");
        dto.TypeName.ShouldBe("General");
        dto.PostedBy.ShouldBe(postedBy);
        dto.SubmittedBy.ShouldBe("actual-account-id");
        dto.SubmittedByName.ShouldBe("Actual Poster");
        dto.PostedByOverridden.ShouldBe(overridden);
    }
}
