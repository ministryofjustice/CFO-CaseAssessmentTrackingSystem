using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.Announcements.EventHandlers;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementNotificationTests
{
    [TestCase(AnnouncementAudience.AllStaff, 2)]
    [TestCase(AnnouncementAudience.InternalStaff, 1)]
    public async Task PublicationNotifiesOnlyActiveAccountsInTheAudience(AnnouncementAudience audience, int count)
    {
        var users = new Mock<IUserService>();
        users.SetupGet(x => x.DataSource).Returns(
        [
            new ApplicationUserDto { Id = "internal", UserName = "staff@justice.gov.uk", Status = UserStatus.Active, LockoutEnd = DateTimeOffset.MaxValue },
            new ApplicationUserDto { Id = "provider", UserName = "staff@provider.org", Status = UserStatus.Active },
            new ApplicationUserDto { Id = "inactive", UserName = "inactive@justice.gov.uk", Status = UserStatus.Inactive },
            new ApplicationUserDto { Id = "left", UserName = "left@justice.gov.uk", Status = UserStatus.Left },
            new ApplicationUserDto { Id = "pending", UserName = "pending@justice.gov.uk", Status = UserStatus.PendingActivation },
            new ApplicationUserDto { Id = "review", UserName = "review@justice.gov.uk", Status = UserStatus.MarkAsInReview }
        ]);
        var saved = new List<Notification>();
        var notifications = new Mock<DbSet<Notification>>();
        notifications.Setup(x => x.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((n, _) => saved.Add(n))
            .Returns(new ValueTask<EntityEntry<Notification>>());
        var context = new Mock<IApplicationDbContext>();
        context.SetupGet(x => x.Notifications).Returns(notifications.Object);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context.Object);
        var handler = new NotifyUsersOnAnnouncementPublished(unitOfWork.Object, users.Object);
        var announcement = Announcement.Create("Release Note", audience, "Release", [], "creator");
        announcement.Approve(announcement.Version, "reviewer");
        var published = announcement.DomainEvents.OfType<AnnouncementPublishedDomainEvent>().Single();

        await handler.Handle(published, CancellationToken.None);

        saved.Count.ShouldBe(count);
        saved.ShouldContain(n => n.OwnerId == "internal");
        saved.ShouldAllBe(n => n.Heading == "Announcement published");
        var expectedLink = "/pages/workspace/my-area/announcements"
            + (audience == AnnouncementAudience.InternalStaff ? "?internal=true" : string.Empty);
        saved.ShouldAllBe(n => n.Link == expectedLink);
        if (audience == AnnouncementAudience.InternalStaff)
        {
            saved.ShouldAllBe(n => n.Link!.EndsWith("?internal=true"));
            saved.ShouldNotContain(n => n.OwnerId == "provider");
        }
        else
        {
            saved.ShouldContain(n => n.OwnerId == "provider");
        }
    }
}
