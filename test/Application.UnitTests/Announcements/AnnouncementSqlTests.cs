#nullable enable
using System.Text.RegularExpressions;
using AutoMapper;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Announcements.Commands;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Application.Features.Announcements.Queries;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Infrastructure.Persistence;
using Cfo.Cats.Infrastructure.Persistence.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

[Explicit("Requires CATS_ANNOUNCEMENTS_SQL_SERVER and CATS_ANNOUNCEMENTS_SQL_PASSWORD; creates and removes an isolated test database.")]
[NonParallelizable]
public class AnnouncementSqlTests
{
    private readonly string _databaseName = $"CatsAnnouncementsTest_{Guid.NewGuid():N}";
    private string _masterConnection = string.Empty;
    private string _connection = string.Empty;
    private bool _created;

    [OneTimeSetUp]
    public async Task CreateDatabase()
    {
        var server = Environment.GetEnvironmentVariable("CATS_ANNOUNCEMENTS_SQL_SERVER");
        var password = Environment.GetEnvironmentVariable("CATS_ANNOUNCEMENTS_SQL_PASSWORD");
        Assert.That(server, Is.Not.Null.And.Not.Empty, "Set CATS_ANNOUNCEMENTS_SQL_SERVER to a local SQL Server endpoint.");
        Assert.That(password, Is.Not.Null.And.Not.Empty, "Set CATS_ANNOUNCEMENTS_SQL_PASSWORD for the local SQL Server sa account.");
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = server,
            UserID = "sa",
            Password = password,
            InitialCatalog = "master",
            TrustServerCertificate = true
        };
        _masterConnection = builder.ConnectionString;
        await ExecuteAsync(_masterConnection, $"CREATE DATABASE [{_databaseName}]");
        _created = true;
        builder.InitialCatalog = _databaseName;
        _connection = builder.ConnectionString;
        await ExecuteAsync(_connection, "CREATE SCHEMA [Configuration]");

        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "cats.slnx")))
        {
            directory = directory.Parent;
        }
        Assert.That(directory, Is.Not.Null, "Could not locate the repository SQL-project files.");
        foreach (var name in new[] { "AnnouncementType", "Announcement", "AnnouncementRevision", "AnnouncementLink" })
        {
            var path = Path.Combine(directory!.FullName, "src", "Database", "CatsDb", "Configuration", "Tables", $"{name}.sql");
            var script = await File.ReadAllTextAsync(path);
            foreach (var batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                         .Where(batch => !string.IsNullOrWhiteSpace(batch)))
            {
                await ExecuteAsync(_connection, batch);
            }
        }
    }

    [SetUp]
    public Task ClearAnnouncements() => ExecuteAsync(_connection,
        "DELETE FROM [Configuration].[Announcement]; DELETE FROM [Configuration].[AnnouncementType]");

    [OneTimeTearDown]
    public async Task DropDatabase()
    {
        if (_created)
        {
            using var poolConnection = new SqlConnection(_connection);
            SqlConnection.ClearPool(poolConnection);
            await ExecuteAsync(_masterConnection,
                $"ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}]");
        }
    }

    private static async Task ExecuteAsync(string connectionString, string sql)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }

    private ApplicationDbContext Context()
        => new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(_connection).Options);

    [Test]
    public async Task NewTypesArePersistedListedAndAvailableToSaveAnnouncements()
    {
        await using var context = Context();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        var add = new AddAnnouncementType.Handler(unitOfWork.Object);
        (await add.Handle(new AddAnnouncementType.Command { Name = " Team Update " }, CancellationToken.None))
            .Succeeded.ShouldBeTrue();
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var types = await new GetAnnouncementTypes.Handler(unitOfWork.Object)
            .Handle(new GetAnnouncementTypes.Query(), CancellationToken.None);
        types.Data!.Select(x => x.Name).ShouldBe(new[] { "Team Update" });
        types.Data!.Single().Id.ShouldNotBe(Guid.Empty);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("manager");
        var users = new Mock<IUserService>();
        users.SetupGet(x => x.DataSource).Returns(
            [new ApplicationUserDto { Id = "manager", UserName = "manager@justice.gov.uk", DisplayName = "Manager Name" }]);
        var repository = new AnnouncementRepository(unitOfWork.Object);
        var save = new SaveAnnouncement.Handler(repository, currentUser.Object, users.Object);
        var result = await save.Handle(new SaveAnnouncement.Command
        {
            TypeName = "team update", Audience = AnnouncementAudience.AllStaff,
            Title = "New update", PostedBy = "Manager Name", Description = "Update description"
        }, CancellationToken.None);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        (await context.Announcements.SingleAsync(x => x.Id == result.Data))
            .PendingRevision!.TypeName.ShouldBe("Team Update");
        await Should.ThrowAsync<NotFoundException>(() =>
            repository.GetTypeNameAsync("Unknown", CancellationToken.None));
    }

    [Test]
    public async Task DuplicateTypesAreRejectedIncludingCaseAndWhitespaceVariations()
    {
        await using var context = Context();
        context.AnnouncementTypes.Add(AnnouncementType.Create("Team Update"));
        await context.SaveChangesAsync();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        await Should.ThrowAsync<ConflictException>(() => new AddAnnouncementType.Handler(unitOfWork.Object)
            .Handle(new AddAnnouncementType.Command { Name = " team update " }, CancellationToken.None));
        context.AnnouncementTypes.Add(AnnouncementType.Create("TEAM UPDATE"));
        await Should.ThrowAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Test]
    public async Task DefaultTypesAreSeededOnlyWhenTheTypeTableIsEmpty()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "cats.slnx")))
        {
            directory = directory.Parent;
        }
        var script = await File.ReadAllTextAsync(Path.Combine(directory!.FullName, "src", "DatabaseSeeding",
            "Scripts", "0074_Announcement_Types.sql"));
        await ExecuteAsync(_connection, script);
        await ExecuteAsync(_connection, script);
        await using var context = Context();
        (await context.AnnouncementTypes.OrderBy(x => x.Name).Select(x => x.Name).ToArrayAsync())
            .ShouldBe(new[] { "General", "Release Note", "Urgent" });
        await ExecuteAsync(_connection,
            "UPDATE [Configuration].[AnnouncementType] SET [Name] = 'Renamed General' WHERE [Name] = 'General'; " +
            "DELETE FROM [Configuration].[AnnouncementType] WHERE [Name] = 'Urgent'");
        await ExecuteAsync(_connection, script);
        (await context.AnnouncementTypes.OrderBy(x => x.Name).Select(x => x.Name).ToArrayAsync())
            .ShouldBe(new[] { "Release Note", "Renamed General" });
        await ExecuteAsync(_connection, "DELETE FROM [Configuration].[AnnouncementType]");
        await ExecuteAsync(_connection, script);
        (await context.AnnouncementTypes.OrderBy(x => x.Name).Select(x => x.Name).ToArrayAsync())
            .ShouldBe(new[] { "General", "Release Note", "Urgent" });
    }

    [Test]
    public async Task RenamingAndDeletingTypesPreserveExistingRevisions()
    {
        await using var context = Context();
        var type = AnnouncementType.Create("Team Update");
        var announcement = Announcement.Create(type.Name, AnnouncementAudience.AllStaff, "Initial", [], "creator");
        announcement.Approve(announcement.Version, "reviewer");
        announcement.Edit(announcement.Version, type.Name, AnnouncementAudience.AllStaff, "Pending", [], "editor");
        context.Add(type);
        context.Add(announcement);
        await context.SaveChangesAsync();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        (await new RenameAnnouncementType.Handler(unitOfWork.Object).Handle(new RenameAnnouncementType.Command
        {
            Id = type.Id, OriginalName = "Team Update", Name = " News "
        }, CancellationToken.None)).Succeeded.ShouldBeTrue();
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var query = new GetAnnouncementTypes.Handler(unitOfWork.Object);
        (await query.Handle(new GetAnnouncementTypes.Query(), CancellationToken.None)).Data!
            .ShouldHaveSingleItem().Name.ShouldBe("News");
        var repository = new AnnouncementRepository(unitOfWork.Object);
        (await repository.GetTypeNameAsync("news", CancellationToken.None)).ShouldBe("News");
        await Should.ThrowAsync<NotFoundException>(() => repository.GetTypeNameAsync("Team Update", CancellationToken.None));

        (await new DeleteAnnouncementType.Handler(unitOfWork.Object).Handle(new DeleteAnnouncementType.Command
        {
            Id = type.Id, OriginalName = "News"
        }, CancellationToken.None)).Succeeded.ShouldBeTrue();
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        (await query.Handle(new GetAnnouncementTypes.Query(), CancellationToken.None)).Data!.ShouldBeEmpty();
        var retained = await context.Announcements.SingleAsync(x => x.Id == announcement.Id);
        retained.Revisions.Count.ShouldBe(2);
        retained.PublishedRevision!.TypeName.ShouldBe("Team Update");
        retained.PendingRevision!.TypeName.ShouldBe("Team Update");
        await Should.ThrowAsync<NotFoundException>(() => repository.GetTypeNameAsync("News", CancellationToken.None));
    }

    [Test]
    public async Task RenamingRejectsDuplicateNamesButAllowsCaseOnlyChanges()
    {
        await using var context = Context();
        var type = AnnouncementType.Create("General");
        context.Add(type);
        context.Add(AnnouncementType.Create("News"));
        await context.SaveChangesAsync();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        var handler = new RenameAnnouncementType.Handler(unitOfWork.Object);
        await Should.ThrowAsync<ConflictException>(() => handler.Handle(new RenameAnnouncementType.Command
        {
            Id = type.Id, OriginalName = "General", Name = " news "
        }, CancellationToken.None));
        (await handler.Handle(new RenameAnnouncementType.Command
        {
            Id = type.Id, OriginalName = "General", Name = "GENERAL"
        }, CancellationToken.None)).Succeeded.ShouldBeTrue();
        await context.SaveChangesAsync();
        type.Name.ShouldBe("GENERAL");
    }

    [Test]
    public async Task StaleAndMissingTypeManagementRequestsAreRejected()
    {
        await using var context = Context();
        var type = AnnouncementType.Create("News");
        context.Add(type);
        await context.SaveChangesAsync();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        var rename = new RenameAnnouncementType.Handler(unitOfWork.Object);
        var delete = new DeleteAnnouncementType.Handler(unitOfWork.Object);
        await Should.ThrowAsync<ConflictException>(() => rename.Handle(new RenameAnnouncementType.Command
        {
            Id = type.Id, OriginalName = "Old name", Name = "Update"
        }, CancellationToken.None));
        await Should.ThrowAsync<ConflictException>(() => delete.Handle(new DeleteAnnouncementType.Command
        {
            Id = type.Id, OriginalName = "Old name"
        }, CancellationToken.None));
        await Should.ThrowAsync<NotFoundException>(() => rename.Handle(new RenameAnnouncementType.Command
        {
            Id = Guid.NewGuid(), OriginalName = "News", Name = "Update"
        }, CancellationToken.None));
        await Should.ThrowAsync<NotFoundException>(() => delete.Handle(new DeleteAnnouncementType.Command
        {
            Id = Guid.NewGuid(), OriginalName = "News"
        }, CancellationToken.None));
        (await context.AnnouncementTypes.SingleAsync()).Name.ShouldBe("News");
    }

    [Test]
    public async Task ConcurrentRenamePreventsAStaleDelete()
    {
        await using var initial = Context();
        var type = AnnouncementType.Create("General");
        initial.Add(type);
        await initial.SaveChangesAsync();
        await using var first = Context();
        await using var second = Context();
        var firstCopy = await first.AnnouncementTypes.SingleAsync(x => x.Id == type.Id);
        var secondCopy = await second.AnnouncementTypes.SingleAsync(x => x.Id == type.Id);
        firstCopy.Rename("News");
        second.AnnouncementTypes.Remove(secondCopy);
        await first.SaveChangesAsync();
        await Should.ThrowAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());
        initial.ChangeTracker.Clear();
        (await initial.AnnouncementTypes.SingleAsync()).Name.ShouldBe("News");
    }

    private static Announcement Published(AnnouncementAudience audience)
    {
        var announcement = Announcement.Create("General", audience, "Published text",
            [new AnnouncementLink("https://example.com", "Guidance")], "creator");
        announcement.Approve(announcement.Version, "reviewer");
        return announcement;
    }

    [Test]
    public async Task ReaderAndManagementQueriesEnforceAudienceAndPublicationState()
    {
        await using var context = Context();
        var shared = Published(AnnouncementAudience.AllStaff);
        var changing = Published(AnnouncementAudience.AllStaff);
        changing.Edit(changing.Version, "Release Note", AnnouncementAudience.InternalStaff,
            "Pending internal changes", [], "editor");
        var internalAnnouncement = Published(AnnouncementAudience.InternalStaff);
        var draft = Announcement.Create("General", AnnouncementAudience.AllStaff, "Draft", [], "creator");
        var deleted = Published(AnnouncementAudience.AllStaff);
        deleted.Delete(deleted.Version, "manager");
        context.AddRange(shared, changing, internalAnnouncement, draft, deleted);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("reader");
        var user = new ApplicationUserDto { Id = "reader", UserName = "reader@provider.org" };
        var users = new Mock<IUserService>();
        users.SetupGet(x => x.DataSource).Returns(() => new[] { user });
        var mapper = new MapperConfiguration(c => c.AddMaps(typeof(AnnouncementDto).Assembly)).CreateMapper();
        var reader = new GetAnnouncements.Handler(unitOfWork.Object, currentUser.Object, users.Object, mapper);
        var management = new GetAnnouncementsForManagement.Handler(unitOfWork.Object, currentUser.Object, users.Object, mapper);

        var sharedResult = await reader.Handle(new GetAnnouncements.Query(), CancellationToken.None);
        var sharedData = sharedResult.Data;
        sharedData.ShouldNotBeNull();
        sharedData.Select(a => a.Id).ShouldBe(new[] { shared.Id, changing.Id }, ignoreOrder: true);
        sharedData.ShouldAllBe(a => a.Pending == null);
        sharedData.Single(a => a.Id == changing.Id).Published!.Description.ShouldBe("Published text");
        sharedData.Single(a => a.Id == shared.Id).Published!.Links.Single().DisplayName.ShouldBe("Guidance");
        sharedData.ShouldNotContain(a => a.Id == deleted.Id);

        var externalManagement = await management.Handle(new GetAnnouncementsForManagement.Query(), CancellationToken.None);
        externalManagement.Data!.Select(a => a.Id).ShouldBe(new[] { shared.Id, draft.Id }, ignoreOrder: true);
        user.UserName = "reader@justice.gov.uk";
        var internalResult = await reader.Handle(new GetAnnouncements.Query
            { Audience = AnnouncementAudience.InternalStaff }, CancellationToken.None);
        internalResult.Data!.ShouldHaveSingleItem().Id.ShouldBe(internalAnnouncement.Id);
        var internalManagement = await management.Handle(new GetAnnouncementsForManagement.Query(), CancellationToken.None);
        internalManagement.Data!.Length.ShouldBe(4);
        internalManagement.Data.Single(a => a.Id == changing.Id).Pending!.Description.ShouldBe("Pending internal changes");
    }

    [Test]
    public async Task HistoryRetainsEveryPublicationAfterNewAnnouncementsArePublished()
    {
        await using var context = Context();
        var older = Published(AnnouncementAudience.AllStaff);
        var newer = Published(AnnouncementAudience.AllStaff);
        var internalAnnouncement = Published(AnnouncementAudience.InternalStaff);
        context.AddRange(older, newer, internalAnnouncement);
        context.Entry(older.PublishedRevision!).Property(x => x.PublishedAt).CurrentValue = DateTime.UtcNow.AddYears(-2);
        context.Entry(newer.PublishedRevision!).Property(x => x.PublishedAt).CurrentValue = DateTime.UtcNow.AddDays(-60);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("reader");
        var users = new Mock<IUserService>();
        users.SetupGet(x => x.DataSource).Returns(
            [new ApplicationUserDto { Id = "reader", UserName = "reader@provider.org" }]);
        var mapper = new MapperConfiguration(c => c.AddMaps(typeof(AnnouncementDto).Assembly)).CreateMapper();
        var handler = new GetAnnouncements.Handler(unitOfWork.Object, currentUser.Object, users.Object, mapper);

        var history = await handler.Handle(new GetAnnouncements.Query(), CancellationToken.None);
        history.Data!.Select(a => a.Id).ShouldBe(new[] { newer.Id, older.Id });
        history.Data!.ShouldAllBe(a => a.Pending == null && a.Published!.Audience == AnnouncementAudience.AllStaff);

        var latest = Published(AnnouncementAudience.AllStaff);
        context.Add(latest);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var updatedHistory = await handler.Handle(new GetAnnouncements.Query(), CancellationToken.None);
        updatedHistory.Data!.Select(a => a.Id).ShouldBe(new[] { latest.Id, newer.Id, older.Id });
        updatedHistory.Data!.ShouldNotContain(a => a.Id == internalAnnouncement.Id);
    }

    [Test]
    public async Task ConcurrentApprovalsRejectTheSecondSave()
    {
        await using var initial = Context();
        var announcement = Announcement.Create("General", AnnouncementAudience.AllStaff, "Text", [], "creator");
        initial.Add(announcement);
        await initial.SaveChangesAsync();

        await using var first = Context();
        await using var second = Context();
        var firstCopy = await first.Announcements.SingleAsync(a => a.Id == announcement.Id);
        var secondCopy = await second.Announcements.SingleAsync(a => a.Id == announcement.Id);
        firstCopy.Approve(firstCopy.Version, "first-reviewer");
        secondCopy.Approve(secondCopy.Version, "second-reviewer");
        await first.SaveChangesAsync();
        await Should.ThrowAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());

        await using var verification = Context();
        var saved = await verification.Announcements.SingleAsync(a => a.Id == announcement.Id);
        saved.PublishedRevision!.ApprovedBy.ShouldBe("first-reviewer");
        saved.PendingRevision.ShouldBeNull();
    }

    [Test]
    public async Task PublishedAndPendingVersionsSurviveReloadAndDeletionRetainsHistory()
    {
        await using var context = Context();
        var announcement = Announcement.Create("General", AnnouncementAudience.AllStaff,
            "Published text", [], "creator", "Original title", "CFO Team", "Original Poster");
        announcement.Approve(announcement.Version, "reviewer");
        context.Add(announcement);
        await context.SaveChangesAsync();
        announcement.Edit(announcement.Version, "Release Note", AnnouncementAudience.AllStaff,
            new string('a', AnnouncementConstants.DescriptionMaximumLength), [], "editor",
            "Edited title", "Release Team", "Editor Name");
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var saved = await context.Announcements.SingleAsync(a => a.Id == announcement.Id);
        saved.PublishedRevision!.Description.ShouldBe("Published text");
        saved.PendingRevision!.Description.Length.ShouldBe(AnnouncementConstants.DescriptionMaximumLength);
        saved.PublishedRevision.Title.ShouldBe("Original title");
        saved.PublishedRevision.PostedBy.ShouldBe("CFO Team");
        saved.PendingRevision.Title.ShouldBe("Edited title");
        saved.PendingRevision.PostedBy.ShouldBe("Release Team");
        saved.PendingRevision.SubmittedByName.ShouldBe("Editor Name");
        saved.Approve(saved.Version, "reviewer");
        await context.SaveChangesAsync();
        saved.Delete(saved.Version, "manager");
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var deleted = await context.Announcements.SingleAsync(a => a.Id == announcement.Id);
        deleted.DeletedBy.ShouldBe("manager");
        deleted.Revisions.Count.ShouldBe(2);
        deleted.PublishedRevision!.TypeName.ShouldBe("Release Note");
    }

    [Test]
    public async Task ManagementReturnsAllRevisionsWithTheirOwnAuthorsEditorsAndApprovals()
    {
        await using var context = Context();
        var announcement = Announcement.Create("General", AnnouncementAudience.AllStaff,
            "Initial", [], "creator", "Initial title", "CFO Team", "Creator Name");
        announcement.Approve(announcement.Version, "first-reviewer");
        announcement.Edit(announcement.Version, "Release Note", AnnouncementAudience.AllStaff,
            "Second", [], "editor", "Second title", "Editor Name", "Editor Name");
        announcement.Approve(announcement.Version, "second-reviewer");
        announcement.Edit(announcement.Version, "General", AnnouncementAudience.AllStaff,
            "Pending", [], "last-editor", "Pending title", "CFO Team", "Last Editor");
        context.Add(announcement);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context);
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("manager");
        var users = new Mock<IUserService>();
        users.SetupGet(x => x.DataSource).Returns(
            [new ApplicationUserDto { Id = "manager", UserName = "manager@justice.gov.uk" }]);
        var mapper = new MapperConfiguration(c => c.AddMaps(typeof(AnnouncementDto).Assembly)).CreateMapper();
        var management = new GetAnnouncementsForManagement.Handler(unitOfWork.Object, currentUser.Object, users.Object, mapper);
        var result = await management.Handle(new GetAnnouncementsForManagement.Query(), CancellationToken.None);
        var dto = result.Data!.ShouldHaveSingleItem();
        dto.History.Length.ShouldBe(3);
        dto.History.Select(r => r.Title).ShouldBe(new[] { "Initial title", "Second title", "Pending title" });
        dto.History.Select(r => r.ApprovedBy).ShouldBe(new[] { "first-reviewer", "second-reviewer", null });
        dto.History.Select(r => r.SubmittedBy).ShouldBe(new[] { "creator", "editor", "last-editor" });
        dto.History.Select(r => r.PostedByOverridden).ShouldBe(new[] { true, false, true });
        dto.Published!.Title.ShouldBe("Second title");
        dto.Pending!.Title.ShouldBe("Pending title");

        var reader = new GetAnnouncements.Handler(unitOfWork.Object, currentUser.Object, users.Object, mapper);
        var readerResult = await reader.Handle(new GetAnnouncements.Query(), CancellationToken.None);
        readerResult.Data!.ShouldHaveSingleItem().History.ShouldBeEmpty();
    }
}
