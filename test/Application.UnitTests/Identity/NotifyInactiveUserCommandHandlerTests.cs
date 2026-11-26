using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Identity.MessageBus;
using Cfo.Cats.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Identity;

public class NotifyInactiveUserCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenSendingFails_ShouldPropagateFailureForRebusRetry()
    {
        var failure = new InvalidOperationException("Email unavailable");
        var communications = new Mock<ICommunicationsService>();
        communications.Setup(service => service.SendAccountDeactivationEmail("user@example.com"))
            .ThrowsAsync(failure);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(work => work.RollbackTransactionAsync()).Returns(Task.CompletedTask);
        var handler = new NotifyInactiveUserCommandHandler(unitOfWork.Object, communications.Object,
            NullLogger<NotifyInactiveUserCommandHandler>.Instance);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.Handle(new NotifyInactiveUserCommand("user@example.com")));

        exception.ShouldBeSameAs(failure);
        unitOfWork.Verify(work => work.RollbackTransactionAsync(), Times.Once);
    }

    [Test]
    public async Task Handle_WhenSendingSucceeds_ShouldSendOnce()
    {
        var communications = new Mock<ICommunicationsService>();
        communications.Setup(service => service.SendAccountDeactivationEmail(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new NotifyInactiveUserCommandHandler(unitOfWork.Object, communications.Object,
            NullLogger<NotifyInactiveUserCommandHandler>.Instance);

        await handler.Handle(new NotifyInactiveUserCommand("user@example.com"));

        communications.Verify(service => service.SendAccountDeactivationEmail("user@example.com"), Times.Once);
        unitOfWork.Verify(work => work.RollbackTransactionAsync(), Times.Never);
    }

    [Test]
    public async Task SendAccountDeactivationEmail_WhenClientCreationFails_ShouldPropagateFailure()
    {
        var failure = new InvalidOperationException("Notify configuration unavailable");
        var options = new Mock<IOptions<NotifyOptions>>();
        options.SetupGet(settings => settings.Value).Throws(failure);
        var service = new CommunicationsService(options.Object, NullLogger<CommunicationsService>.Instance);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            service.SendAccountDeactivationEmail("user@example.com"));

        exception.ShouldBeSameAs(failure);
    }
}