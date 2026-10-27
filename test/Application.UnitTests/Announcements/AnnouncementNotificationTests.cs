using System.Collections;
using System.Linq.Expressions;
using System.Text.Json;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Announcements.EventHandlers;
using Cfo.Cats.Application.Features.Announcements.IntegrationEventHandlers;
using Cfo.Cats.Application.Features.Announcements.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Notifications;
using Cfo.Cats.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementNotificationTests
{
    [Test]
    public async Task PublicationQueuesAnIntegrationEventWithoutCreatingNotifications()
    {
        var saved = new List<OutboxMessage>();
        var outbox = new Mock<DbSet<OutboxMessage>>();
        outbox.Setup(x => x.AddAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()))
            .Callback<OutboxMessage, CancellationToken>((m, _) => saved.Add(m))
            .Returns(new ValueTask<EntityEntry<OutboxMessage>>());
        var context = new Mock<IApplicationDbContext>(MockBehavior.Strict);
        context.SetupGet(x => x.OutboxMessages).Returns(outbox.Object);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context.Object);
        var announcement = Announcement.Create("General", AnnouncementAudience.InternalStaff, "Text", [], "creator");
        announcement.Approve(announcement.Version, "reviewer");
        var published = announcement.DomainEvents.OfType<AnnouncementPublishedDomainEvent>().Single();

        await new PublishAnnouncementPublishedEventHandler(unitOfWork.Object)
            .Handle(published, CancellationToken.None);

        saved.Count.ShouldBe(1);
        saved[0].Type.ShouldBe(typeof(AnnouncementPublishedIntegrationEvent).FullName);
        var message = JsonSerializer.Deserialize<AnnouncementPublishedIntegrationEvent>(saved[0].Content)!;
        message.AnnouncementId.ShouldBe(announcement.Id);
        message.RevisionId.ShouldBe(published.Revision.Id);
        message.Title.ShouldBe(published.Revision.Title);
        message.Audience.ShouldBe(AnnouncementAudience.InternalStaff);
        message.OccurredOn.ShouldBe(published.DateOccurred.UtcDateTime);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestCase(AnnouncementAudience.AllStaff, 4)]
    [TestCase(AnnouncementAudience.InternalStaff, 1)]
    public async Task ConsumerNotifiesOnlyActiveAccountsUsingRecipientEmails(AnnouncementAudience audience, int count)
    {
        var data = new AsyncQuery<ApplicationUser>(
        [
            new() { Id = "internal", Email = "STAFF@JUSTICE.GOV.UK", UserName = "different@provider.org", Status = UserStatus.Active, LockoutEnd = DateTimeOffset.MaxValue },
            new() { Id = "provider", Email = "staff@provider.org", UserName = "different@justice.gov.uk", Status = UserStatus.Active },
            new() { Id = "lookalike", Email = "staff@justice.gov.uk.provider.org", Status = UserStatus.Active },
            new() { Id = "missing", Email = null, Status = UserStatus.Active },
            new() { Id = "inactive", Email = "inactive@justice.gov.uk", Status = UserStatus.Inactive },
            new() { Id = "left", Email = "left@justice.gov.uk", Status = UserStatus.Left },
            new() { Id = "pending", Email = "pending@justice.gov.uk", Status = UserStatus.PendingActivation },
            new() { Id = "review", Email = "review@justice.gov.uk", Status = UserStatus.MarkAsInReview }
        ]);
        var users = new Mock<DbSet<ApplicationUser>>();
        users.As<IQueryable<ApplicationUser>>().SetupGet(x => x.Provider).Returns(data);
        users.As<IQueryable<ApplicationUser>>().SetupGet(x => x.Expression).Returns(data.Expression);
        users.As<IQueryable<ApplicationUser>>().SetupGet(x => x.ElementType).Returns(typeof(ApplicationUser));
        users.As<IQueryable<ApplicationUser>>().Setup(x => x.GetEnumerator()).Returns(() => data.GetEnumerator());
        var saved = new List<Notification>();
        var notifications = new Mock<DbSet<Notification>>();
        notifications.Setup(x => x.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((n, _) => saved.Add(n))
            .Returns(new ValueTask<EntityEntry<Notification>>());
        var context = new Mock<IApplicationDbContext>();
        context.SetupGet(x => x.Users).Returns(users.Object);
        context.SetupGet(x => x.Notifications).Returns(notifications.Object);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.DbContext).Returns(context.Object);
        var message = new AnnouncementPublishedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(),
            "Service update", audience, DateTime.UtcNow);

        await new NotifyUsersOnAnnouncementPublishedConsumer(unitOfWork.Object).Handle(message);

        saved.Count.ShouldBe(count);
        saved.ShouldContain(n => n.OwnerId == "internal");
        saved.ShouldAllBe(n => n.Heading == "Announcement published" && n.Details == "New Announcement: Service update");
        var expectedLink = "/pages/workspace/my-area/announcements"
            + (audience == AnnouncementAudience.InternalStaff ? "?internal=true" : string.Empty);
        saved.ShouldAllBe(n => n.Link == expectedLink);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed class AsyncQuery<T> : IQueryable<T>, IAsyncEnumerable<T>, IQueryProvider
    {
        private readonly IQueryable<T> _query;
        public AsyncQuery(IEnumerable<T> items) => _query = items.AsQueryable();
        private AsyncQuery(IQueryable<T> query) => _query = query;
        public Type ElementType => typeof(T);
        public Expression Expression => _query.Expression;
        public IQueryProvider Provider => this;
        public IEnumerator<T> GetEnumerator() => _query.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IQueryable CreateQuery(Expression expression) => throw new NotSupportedException();
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new AsyncQuery<TElement>(_query.Provider.CreateQuery<TElement>(expression));
        public object Execute(Expression expression) => _query.Provider.Execute(expression)!;
        public TResult Execute<TResult>(Expression expression) => _query.Provider.Execute<TResult>(expression);
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var item in _query)
            {
                yield return item;
            }
            await Task.CompletedTask;
        }
    }
}
