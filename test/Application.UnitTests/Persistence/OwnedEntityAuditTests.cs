using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities;
using Cfo.Cats.Domain.Entities.Bios;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Infrastructure.Persistence;
using Cfo.Cats.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Persistence;

public class OwnedEntityAuditTests
{
    private ApplicationDbContext _db = null!;

    [SetUp]
    public async Task SetUp()
    {
        var user = new Mock<ICurrentUserService>();
        user.Setup(u => u.UserId).Returns("user1");
        user.Setup(u => u.TenantId).Returns("1.");
        var dateTime = new Mock<IDateTime>();
        var clock = DateTime.UtcNow;
        dateTime.Setup(d => d.Now).Returns(() => clock = clock.AddSeconds(1));

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditableEntityInterceptor(user.Object, dateTime.Object))
            .Options;
        _db = new ApplicationDbContext(options);

        var participant = Participant.CreateFrom("P1", "A", null, "B", null, new DateTime(1990, 1, 1), null, "src", null, 1, null, null, null);
        participant.UpdateSupervisor(Supervisor.CreateFrom("Sup", "1", "2", "a@b.c", "addr"));
        participant.UpdatePersonalDetail(PersonalDetail.CreateFrom("old", "p", "t", "AB123456C", "x"));
        participant.AddConsent(DateTime.UtcNow.AddDays(-5), Guid.NewGuid());
        _db.Participants.Add(participant);
        await _db.SaveChangesAsync();

        _db.AuditTrails.RemoveRange(_db.AuditTrails);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
    }

    [TearDown]
    public async Task TearDown() => await _db.DisposeAsync();

    private Task<Participant> LoadParticipant() =>
        _db.Participants.IgnoreAutoIncludes().IgnoreQueryFilters().SingleAsync();

    [Test]
    public async Task EditingSupervisor_IsAudited()
    {
        var participant = await LoadParticipant();
        participant.Supervisor!.Name = "Changed";
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Supervisor));
        audit.AffectedColumns.ShouldBe([nameof(Supervisor.Name)]);
        audit.NewValues![nameof(Supervisor.Name)].ShouldBe("Changed");
    }

    [Test]
    public async Task ReplacingPersonalDetail_IsAuditedAgainstOwner()
    {
        var participant = await LoadParticipant();
        participant.UpdatePersonalDetail(PersonalDetail.CreateFrom("new", "p", "t", "AB123456C", "x"));
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Participant));
        audit.AffectedColumns.ShouldNotBeNull().ShouldContain("PersonalDetail.PreferredNames");

        audit.OldValues.ShouldNotBeNull()["PersonalDetail.PreferredNames"].ShouldBe("old");
        audit.NewValues.ShouldNotBeNull()["PersonalDetail.PreferredNames"].ShouldBe("new");
    }

    [Test]
    public async Task RemovingPersonalDetail_IsAuditedAgainstOwner()
    {
        var participant = await LoadParticipant();
        participant.UpdatePersonalDetail(null);
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Participant));
        audit.OldValues!["PersonalDetail.PreferredNames"].ShouldBe("old");
    }

    [Test]
    public async Task AddingExternalIdentifier_IsAuditedAgainstOwner()
    {
        var participant = await LoadParticipant();
        participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create("A1234BC", ExternalIdentifierType.NomisNumber));
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Participant));
        audit.NewValues!.Keys.ShouldContain(k => k.StartsWith("ExternalIdentifiers[") && k.EndsWith("].Value"));
        audit.NewValues.Values.ShouldContain("A1234BC");
    }

    [Test]
    public async Task ReplacingExternalIdentifier_RecordsOldAndNewValues()
    {
        var participant = await LoadParticipant();
        participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create("A1234BC", ExternalIdentifierType.NomisNumber));
        await _db.SaveChangesAsync();
        _db.AuditTrails.RemoveRange(_db.AuditTrails);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        participant = await _db.Participants.IgnoreAutoIncludes().IgnoreQueryFilters()
            .Include(p => p.ExternalIdentifiers).SingleAsync();
        participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create("Z9999ZZ", ExternalIdentifierType.NomisNumber));
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Participant));
        audit.OldValues!.Values.ShouldContain("A1234BC");
        audit.NewValues!.Values.ShouldContain("Z9999ZZ");
    }

    [Test]
    public async Task NewNestedCollectionItems_GetDistinctPaths()
    {
        var announcement = Announcement.Create(
            "Info",
            AnnouncementAudience.AllStaff,
            "desc",
            [new AnnouncementLink("https://a.example", "A"), new AnnouncementLink("https://b.example", "B")],
            "user1");
        _db.Announcements.Add(announcement);
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Announcement));
        var newValues = audit.NewValues.ShouldNotBeNull();
        var revisionId = announcement.Revisions.Single().Id;

        newValues[$"Revisions[{revisionId}].Links[1].Url"].ShouldBe("https://a.example");
        newValues[$"Revisions[{revisionId}].Links[2].Url"].ShouldBe("https://b.example");
    }

    [Test]
    public async Task CreateRow_LeavesAffectedColumnsEmptyForFoldedProperties()
    {
        var participant = Participant.CreateFrom("P2", "A", null, "B", null, new DateTime(1990, 1, 1), null, "src", null, 1, null, null, null);
        participant.UpdateSupervisor(Supervisor.CreateFrom("Sup", "1", "2", "a@b.c", "addr"));
        participant.UpdatePersonalDetail(PersonalDetail.CreateFrom("pn", "p", "t", "AB123456C", "x"));
        _db.Participants.Add(participant);
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Participant) && a.AuditType == AuditType.Create);
        audit.NewValues.ShouldNotBeNull().ShouldContainKey("PersonalDetail.PreferredNames");
        audit.AffectedColumns.ShouldNotBeNull().ShouldBeEmpty();
    }

    [Test]
    public async Task ReplacingPersonalDetail_DoesNotDuplicateAffectedColumns()
    {
        var participant = await LoadParticipant();
        participant.UpdatePersonalDetail(PersonalDetail.CreateFrom("new", "p", "t", "AB123456C", "x"));
        await _db.SaveChangesAsync();

        var audit = await _db.AuditTrails.SingleAsync(a => a.TableName == nameof(Participant));
        audit.AffectedColumns.ShouldNotBeNull().ShouldBe(audit.AffectedColumns.Distinct().ToList());
    }

    [Test]
    public async Task ShallowAuditableOwner_ProducesNoAudit()
    {
        var bio = ParticipantBio.Create(Guid.NewGuid(), "P1");
        bio.SetAnswer("Q1", "A1");
        _db.ParticipantBios.Add(bio);
        await _db.SaveChangesAsync();

        (await _db.AuditTrails.AnyAsync()).ShouldBeFalse();
    }

    [Test]
    public async Task EndingConsentLifetime_IsAudited()
    {
        var participant = await LoadParticipant();
        participant.AddConsent(DateTime.UtcNow, Guid.NewGuid());
        await _db.SaveChangesAsync();

        var audits = await _db.AuditTrails.Where(a => a.TableName == nameof(Consent)).ToListAsync();

        var ended = audits.Single(a => a.AuditType == AuditType.Update);
        ended.AffectedColumns.ShouldNotBeNull().ShouldContain("Lifetime.EndDate");

        var created = audits.Single(a => a.AuditType == AuditType.Create);
        created.NewValues.ShouldNotBeNull().ShouldContainKey("Lifetime.StartDate");
    }
}
