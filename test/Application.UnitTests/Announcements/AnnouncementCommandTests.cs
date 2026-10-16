#nullable enable
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Announcements.Commands;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Pipeline;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementCommandTests
{
    private Mock<IAnnouncementRepository> _repository = null!;
    private Mock<ICurrentUserService> _currentUser = null!;
    private Mock<IUserService> _users = null!;
    private ApplicationUserDto _user = null!;
    private SaveAnnouncement.Handler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IAnnouncementRepository>();
        _currentUser = new Mock<ICurrentUserService>();
        _users = new Mock<IUserService>();
        _user = new ApplicationUserDto { Id = "manager", UserName = "manager@provider.org", DisplayName = "Senior Manager" };
        _users.SetupGet(x => x.DataSource).Returns(() => new[] { _user });
        _currentUser.SetupGet(x => x.UserId).Returns("manager");
        _repository.Setup(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repository.Setup(x => x.GetTypeNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, CancellationToken _) => name.Trim());
        _handler = new SaveAnnouncement.Handler(_repository.Object, _currentUser.Object, _users.Object);
    }

    private static SaveAnnouncement.Command Command() => new()
    {
        TypeName = "General",
        Audience = AnnouncementAudience.AllStaff,
        Title = "Service update",
        PostedBy = "Senior Manager",
        Description = "Announcement"
    };

    [Test]
    public async Task CreateSubmitsWithoutPublishing()
    {
        Announcement? saved = null;
        _repository.Setup(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()))
            .Callback<Announcement, CancellationToken>((a, _) => saved = a).Returns(Task.CompletedTask);
        var result = await _handler.Handle(Command(), CancellationToken.None);
        result.Succeeded.ShouldBeTrue();
        saved.ShouldNotBeNull();
        saved.PendingRevision.ShouldNotBeNull();
        saved.PendingRevision.Title.ShouldBe("Service update");
        saved.PendingRevision.PostedBy.ShouldBe("Senior Manager");
        saved.PendingRevision.SubmittedByName.ShouldBe("Senior Manager");
        saved.PublishedRevision.ShouldBeNull();
        result.Data.ShouldBe(saved.Id);
    }

    [Test]
    public async Task SupportAutoApprovalIsExplicit()
    {
        _user.AssignedRoles = [RoleNames.SystemSupport];
        Announcement? saved = null;
        _repository.Setup(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()))
            .Callback<Announcement, CancellationToken>((a, _) => saved = a).Returns(Task.CompletedTask);
        var command = Command();
        command.PublishImmediately = true;
        (await _handler.Handle(command, CancellationToken.None)).Succeeded.ShouldBeTrue();
        saved.ShouldNotBeNull();
        saved.PublishedRevision!.AutoApproved.ShouldBeTrue();
    }

    [Test]
    public void NonSupportCannotForgeImmediatePublication()
    {
        var command = Command();
        command.PublishImmediately = true;
        Should.Throw<ForbiddenException>(async () => await _handler.Handle(command, CancellationToken.None));
        _repository.Verify(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public void NonInternalUserCannotCreateInternalAnnouncement()
    {
        var command = Command();
        command.Audience = AnnouncementAudience.InternalStaff;
        Should.Throw<ForbiddenException>(async () => await _handler.Handle(command, CancellationToken.None));
        _repository.Verify(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task InternalUserCanCreateInternalAnnouncement()
    {
        _user.UserName = "manager@justice.gov.uk";
        _currentUser.SetupGet(x => x.IsInternalUser).Returns(true);
        var command = Command();
        command.Audience = AnnouncementAudience.InternalStaff;
        (await _handler.Handle(command, CancellationToken.None)).Succeeded.ShouldBeTrue();
    }

    [Test]
    public void NonInternalUserCannotChangeAnInternalAnnouncementToAllStaff()
    {
        var announcement = Announcement.Create(
            "General", AnnouncementAudience.InternalStaff, "Internal", [], "creator");
        _repository.Setup(x => x.GetByIdAsync(announcement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(announcement);
        var command = Command();
        command.Id = announcement.Id;
        command.Version = announcement.Version;
        Should.Throw<ForbiddenException>(async () => await _handler.Handle(command, CancellationToken.None));
        announcement.PendingRevision!.Description.ShouldBe("Internal");
    }

    [Test]
    public void InternalApprovalAndDeletionRequireInternalAccess()
    {
        var announcement = Announcement.Create(
            "General", AnnouncementAudience.InternalStaff, "Internal", [], "creator");
        _repository.Setup(x => x.GetByIdAsync(announcement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(announcement);
        var approve = new ApproveAnnouncement.Handler(_repository.Object, _currentUser.Object, _users.Object);
        var delete = new DeleteAnnouncement.Handler(_repository.Object, _currentUser.Object);
        Should.Throw<ForbiddenException>(async () => await approve.Handle(
            new ApproveAnnouncement.Command { Id = announcement.Id, Version = announcement.Version }, CancellationToken.None));
        Should.Throw<ForbiddenException>(async () => await delete.Handle(
            new DeleteAnnouncement.Command { Id = announcement.Id, Version = announcement.Version }, CancellationToken.None));
        announcement.PublishedRevision.ShouldBeNull();
        announcement.DeletedAt.ShouldBeNull();
    }

    [Test]
    public async Task EditingPublishedAnnouncementDoesNotPublishAutomatically()
    {
        var announcement = Announcement.Create(
            "General", AnnouncementAudience.AllStaff, "Original", [], "creator");
        announcement.Approve(announcement.Version, "reviewer");
        announcement.ClearDomainEvents();
        _repository.Setup(x => x.GetByIdAsync(announcement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(announcement);
        var command = Command();
        command.Id = announcement.Id;
        command.Version = announcement.Version;
        (await _handler.Handle(command, CancellationToken.None)).Succeeded.ShouldBeTrue();
        announcement.PublishedRevision!.Description.ShouldBe("Original");
        announcement.PendingRevision!.Description.ShouldBe("Announcement");
        announcement.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void ValidatorAllowsAnnouncementsWithoutLinks()
        => new SaveAnnouncement.Validator().Validate(Command()).IsValid.ShouldBeTrue();

    [Test]
    public async Task SelectedCustomTypeIsStoredUsingItsDatabaseName()
    {
        _repository.Setup(x => x.GetTypeNameAsync("team update", It.IsAny<CancellationToken>()))
            .ReturnsAsync("Team Update");
        Announcement? saved = null;
        _repository.Setup(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()))
            .Callback<Announcement, CancellationToken>((a, _) => saved = a).Returns(Task.CompletedTask);
        var command = Command();
        command.TypeName = "team update";
        await _handler.Handle(command, CancellationToken.None);
        saved.ShouldNotBeNull();
        saved.PendingRevision!.TypeName.ShouldBe("Team Update");
    }

    [Test]
    public void UnknownTypesCannotBeSubmitted()
    {
        _repository.Setup(x => x.GetTypeNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Choose an existing announcement type."));
        Should.Throw<NotFoundException>(async () => await _handler.Handle(Command(), CancellationToken.None));
        _repository.Verify(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CustomAuthorDoesNotChangeRecordedAccountIdentity()
    {
        Announcement? saved = null;
        _repository.Setup(x => x.AddAsync(It.IsAny<Announcement>(), It.IsAny<CancellationToken>()))
            .Callback<Announcement, CancellationToken>((a, _) => saved = a).Returns(Task.CompletedTask);
        var command = Command();
        command.PostedBy = "CFO Team";
        await _handler.Handle(command, CancellationToken.None);
        saved.ShouldNotBeNull();
        saved.PendingRevision!.PostedBy.ShouldBe("CFO Team");
        saved.PendingRevision.SubmittedBy.ShouldBe("manager");
        saved.PendingRevision.SubmittedByName.ShouldBe("Senior Manager");
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task SupportCanApproveFromManagementWithoutEditing(bool ownAnnouncement)
    {
        _user.AssignedRoles = [RoleNames.SystemSupport];
        var announcement = Announcement.Create("General",
            AnnouncementAudience.AllStaff, "Text", [], ownAnnouncement ? "manager" : "creator");
        var pendingId = announcement.PendingRevisionId;
        _repository.Setup(x => x.GetByIdAsync(announcement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(announcement);
        var handler = new ApproveAnnouncement.Handler(_repository.Object, _currentUser.Object, _users.Object);
        var result = await handler.Handle(
            new ApproveAnnouncement.Command { Id = announcement.Id, Version = announcement.Version }, CancellationToken.None);
        result.Succeeded.ShouldBeTrue();
        announcement.PublishedRevisionId.ShouldBe(pendingId);
        announcement.Revisions.Count.ShouldBe(1);
        announcement.PublishedRevision!.ApprovedBy.ShouldBe("manager");
        announcement.PublishedRevision.AutoApproved.ShouldBe(ownAnnouncement);
    }

    [Test]
    public void NonSupportStillCannotApproveOwnAnnouncement()
    {
        var announcement = Announcement.Create("General", AnnouncementAudience.AllStaff, "Text", [], "manager");
        _repository.Setup(x => x.GetByIdAsync(announcement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(announcement);
        var handler = new ApproveAnnouncement.Handler(_repository.Object, _currentUser.Object, _users.Object);
        Should.Throw<Cfo.Cats.Domain.Common.Exceptions.BusinessRuleValidationException>(async () => await handler.Handle(
            new ApproveAnnouncement.Command { Id = announcement.Id, Version = announcement.Version }, CancellationToken.None));
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task OriginalCreatorCanApproveAnotherUsersEditWithoutAnOverride(bool systemSupport)
    {
        _user.AssignedRoles = systemSupport ? [RoleNames.SystemSupport] : [];
        var announcement = Announcement.Create("General",
            AnnouncementAudience.AllStaff, "Original", [], "manager");
        announcement.Approve(announcement.Version, "reviewer");
        announcement.Edit(announcement.Version, "General",
            AnnouncementAudience.AllStaff, "Edited", [], "reviewer");
        _repository.Setup(x => x.GetByIdAsync(announcement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(announcement);
        var handler = new ApproveAnnouncement.Handler(_repository.Object, _currentUser.Object, _users.Object);

        var result = await handler.Handle(
            new ApproveAnnouncement.Command { Id = announcement.Id, Version = announcement.Version }, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        announcement.PublishedRevision!.Description.ShouldBe("Edited");
        announcement.PublishedRevision.ApprovedBy.ShouldBe("manager");
        announcement.PublishedRevision.AutoApproved.ShouldBeFalse();
    }

    [TestCase("", "Name")]
    [TestCase("Title", "")]
    [TestCase(" ", "Name")]
    [TestCase("Title", " ")]
    public void TitleAndAuthorAreRequired(string title, string author)
    {
        var command = Command();
        command.Title = title;
        command.PostedBy = author;
        new SaveAnnouncement.Validator().Validate(command).IsValid.ShouldBeFalse();
    }

    [TestCase("https://example.com/release", true)]
    [TestCase("http://example.com/release", true)]
    [TestCase("javascript:alert(1)", false)]
    [TestCase("file:///etc/passwd", false)]
    [TestCase("/relative/path", false)]
    [TestCase("", false)]
    public void ValidatorRestrictsLinkSchemes(string url, bool expected)
    {
        var command = Command();
        command.Links = [new AnnouncementLinkDto { Url = url }];
        new SaveAnnouncement.Validator().Validate(command).IsValid.ShouldBe(expected);
    }

    [Test]
    public void ValidatorRequiresEditVersion()
    {
        var command = Command();
        command.Id = Guid.NewGuid();
        new SaveAnnouncement.Validator().Validate(command).IsValid.ShouldBeFalse();
        command.Version = Guid.NewGuid();
        new SaveAnnouncement.Validator().Validate(command).IsValid.ShouldBeTrue();
    }

    [Test]
    public void ValidatorEnforcesDescriptionAndLinkLengths()
    {
        var command = Command();
        command.Description = new string('a', AnnouncementConstants.DescriptionMaximumLength + 1);
        command.Links =
        [
            new AnnouncementLinkDto
            {
                Url = "https://example.com/" + new string('a', AnnouncementConstants.UrlMaximumLength),
                DisplayName = new string('a', AnnouncementConstants.DisplayNameMaximumLength + 1)
            }
        ];
        var result = new SaveAnnouncement.Validator().Validate(command);
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
        result.Errors.ShouldContain(e => e.PropertyName == "Links[0].Url");
        result.Errors.ShouldContain(e => e.PropertyName == "Links[0].DisplayName");
    }

    [Test]
    public async Task ConcurrentSaveFailureReturnsAnExplicitRefreshMessage()
    {
        var behavior = new CommandUnhandledExceptionBehaviour<ApproveAnnouncement.Command, Result>(
            new Mock<ILogger<ApproveAnnouncement.Command>>().Object, _currentUser.Object);
        var result = await behavior.Handle(
            new ApproveAnnouncement.Command { Id = Guid.NewGuid(), Version = Guid.NewGuid() },
            () => Task.FromException<Result>(new DbUpdateConcurrencyException("Concurrent approval")),
            CancellationToken.None);
        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("This record has changed. Refresh the page and try again.");
    }
}
