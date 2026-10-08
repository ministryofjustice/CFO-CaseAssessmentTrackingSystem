using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Identity.MessageBus;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Identity.MessageBus;

/// <summary>
/// Regression tests for the "Rebus message silently dropped on failure" finding:
/// previously, <see cref="NotifyInactiveUserCommandHandler"/> caught any exception, rolled back
/// the unit of work, logged it, and then returned normally. In a Rebus message-bus handler this
/// means the message is treated as successfully handled (acked) and is never retried or moved to
/// the error queue - so a genuine failure (e.g. a database error) would be silently lost.
///
/// These tests assert that both unexpected exceptions AND failed sends (a
/// <see cref="Result.Failure(string[])"/> from <see cref="ICommunicationsService"/>, e.g. a blank
/// Notify API key) fail the delivery, so Rebus moves the message to the error queue instead of
/// ack-ing it. Each user is only published once, so an ack'd failure would never be re-sent.
/// </summary>
public class NotifyInactiveUserCommandHandlerTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<ICommunicationsService> _communicationsServiceMock = null!;
    private Mock<ILogger<NotifyInactiveUserCommandHandler>> _loggerMock = null!;
    private NotifyInactiveUserCommandHandler _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _communicationsServiceMock = new Mock<ICommunicationsService>();
        _loggerMock = new Mock<ILogger<NotifyInactiveUserCommandHandler>>();
        _sut = new NotifyInactiveUserCommandHandler(_unitOfWorkMock.Object, _communicationsServiceMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_WhenEmailSendFails_Throws_SoRebusDoesNotAckTheMessage()
    {
        // NotifyAccountDeactivationJob publishes each user only once, so a failed send (e.g. blank
        // Notify:ApiKey or a Notify API error) must fail the delivery and reach the error queue
        // rather than being ack'd and lost.
        _communicationsServiceMock
            .Setup(x => x.SendAccountDeactivationEmail(It.IsAny<string>()))
            .ReturnsAsync(Result.Failure("messaging service is not configured."));

        await Should.ThrowAsync<InvalidOperationException>(() => _sut.Handle(new NotifyInactiveUserCommand("user@example.com")));
    }

    [Test]
    public async Task Handle_WhenEmailSendSucceeds_DoesNotThrow_AndDoesNotRollback()
    {
        _communicationsServiceMock
            .Setup(x => x.SendAccountDeactivationEmail(It.IsAny<string>()))
            .ReturnsAsync(Result.Success());

        await _sut.Handle(new NotifyInactiveUserCommand("user@example.com"));

        _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(), Times.Never);
    }

    [Test]
    public async Task Handle_WhenAnUnexpectedExceptionOccurs_RollsBackAndRethrows()
    {
        _communicationsServiceMock
            .Setup(x => x.SendAccountDeactivationEmail(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        // The message must NOT be silently swallowed: Rebus needs the exception to propagate so
        // it can retry the delivery / move it to the error queue instead of ack-ing a lost
        // notification.
        await Should.ThrowAsync<InvalidOperationException>(() => _sut.Handle(new NotifyInactiveUserCommand("user@example.com")));
    }

    [Test]
    public async Task Handle_WhenAnUnexpectedExceptionOccurs_RollsBackTransactionBeforeRethrowing()
    {
        _communicationsServiceMock
            .Setup(x => x.SendAccountDeactivationEmail(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        try
        {
            await _sut.Handle(new NotifyInactiveUserCommand("user@example.com"));
        }
        catch (InvalidOperationException)
        {
            // expected - asserted in a separate test; here we only care about the rollback side-effect.
        }

        _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
    }
}