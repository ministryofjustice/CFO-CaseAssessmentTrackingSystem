using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Pipeline;
using Cfo.Cats.Application.Pipeline.ValidationSpecifications;
using Cortex.Mediator.Commands;
using Cortex.Mediator.Queries;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common.Mediator;

public class PipelineCancellationTests
{
    [TestCase(true)]
    [TestCase(false)]
    public async Task ExceptionBehaviourPropagatesRequestCancellation(bool command)
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var token = source.Token;

        var exception = await Should.ThrowAsync<OperationCanceledException>(
            () => HandleException(command, () => Task.FromCanceled<Result<string>>(token), token));

        exception.CancellationToken.ShouldBe(token);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ExceptionBehaviourStillReturnsFailureForUnrelatedCancellation(bool command)
    {
        var result = await HandleException(
            command,
            () => Task.FromException<Result<string>>(new OperationCanceledException()),
            CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Unexpected error");
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task SessionBehaviourForwardsTokenToBothSessionOperations(bool command)
    {
        using var source = new CancellationTokenSource();
        var token = source.Token;
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(user => user.UserId).Returns("user");
        var session = new Mock<ISessionService>(MockBehavior.Strict);
        session.Setup(service => service.IsSessionValidAsync("user", token)).ReturnsAsync(true);
        session.Setup(service => service.UpdateActivityAsync("user", token)).Returns(Task.CompletedTask);

        var result = command
            ? await new CommandSessionValidatingBehaviour<TestCommand, Result<string>>(session.Object, currentUser.Object)
                .Handle(new TestCommand(), () => Task.FromResult(Result<string>.Success("loaded")), token)
            : await new QuerySessionValidatingBehaviour<TestQuery, Result<string>>(session.Object, currentUser.Object)
                .Handle(new TestQuery(), () => Task.FromResult(Result<string>.Success("loaded")), token);

        result.Succeeded.ShouldBeTrue();
        session.VerifyAll();
    }

    [Test]
    public async Task AccessValidationForwardsTokenToSpecifications()
    {
        using var source = new CancellationTokenSource();
        var specification = new TokenCheckingSpecification(source.Token);
        var behaviour = new AccessValidationBehaviour<AuditableQuery, Result<string>>(
            [specification],
            NullLogger<AccessValidationBehaviour<AuditableQuery, Result<string>>>.Instance);

        var result = await behaviour.Handle(
            new AuditableQuery(),
            () => Task.FromResult(Result<string>.Success("loaded")),
            source.Token);

        result.Succeeded.ShouldBeTrue();
        specification.WasCalled.ShouldBeTrue();
    }

    [Test]
    public async Task AccessValidationStopsBeforeCheckingSpecificationsWhenCancelled()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var specification = new TokenCheckingSpecification(source.Token);
        var behaviour = new AccessValidationBehaviour<AuditableQuery, Result<string>>(
            [specification],
            NullLogger<AccessValidationBehaviour<AuditableQuery, Result<string>>>.Instance);

        await Should.ThrowAsync<OperationCanceledException>(() => behaviour.Handle(
            new AuditableQuery(),
            () => throw new InvalidOperationException("The handler must not run."),
            source.Token));

        specification.WasCalled.ShouldBeFalse();
    }

    [Test]
    public async Task AuditingDoesNotSwallowRequestCancellation()
    {
        using var source = new CancellationTokenSource();
        var context = new Mock<IApplicationDbContext>();
        context.SetupGet(db => db.Participants)
            .Throws(() => new OperationCanceledException(source.Token));
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(work => work.DbContext).Returns(context.Object);
        var behaviour = new AccessAuditingBehaviour<AuditableQuery, Result<string>>(
            unitOfWork.Object,
            Mock.Of<ICurrentUserService>());

        await Should.ThrowAsync<OperationCanceledException>(() => behaviour.Handle(
            new AuditableQuery(),
            () =>
            {
                source.Cancel();
                return Task.FromResult(Result<string>.Success("loaded"));
            },
            source.Token));
    }

    [Test]
    public async Task TransactionForwardsTokenToBeginDispatchAndCommit()
    {
        using var source = new CancellationTokenSource();
        var token = source.Token;
        var context = Mock.Of<IApplicationDbContext>();
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(work => work.DbContext).Returns(context);
        unitOfWork.Setup(work => work.BeginTransactionAsync(token)).Returns(Task.CompletedTask);
        unitOfWork.Setup(work => work.CommitTransactionAsync(token)).Returns(Task.CompletedTask);
        var dispatcher = new Mock<IDomainEventDispatcher>(MockBehavior.Strict);
        dispatcher.Setup(service => service.DispatchEventsAsync(context, token)).Returns(Task.CompletedTask);
        var behaviour = new QueryTransactionBehaviour<TestQuery, Result<string>>(unitOfWork.Object, dispatcher.Object);

        var result = await behaviour.Handle(
            new TestQuery(),
            () => Task.FromResult(Result<string>.Success("loaded")),
            token);

        result.Succeeded.ShouldBeTrue();
        unitOfWork.VerifyAll();
        dispatcher.VerifyAll();
    }

    [Test]
    public async Task TransactionRollsBackAndPropagatesCancellationWithoutCommitting()
    {
        using var source = new CancellationTokenSource();
        var token = source.Token;
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(work => work.BeginTransactionAsync(token)).Returns(Task.CompletedTask);
        unitOfWork.Setup(work => work.RollbackTransactionAsync()).Returns(Task.CompletedTask);
        var behaviour = new QueryTransactionBehaviour<TestQuery, Result<string>>(
            unitOfWork.Object,
            Mock.Of<IDomainEventDispatcher>());

        await Should.ThrowAsync<OperationCanceledException>(() => behaviour.Handle(
            new TestQuery(),
            () =>
            {
                source.Cancel();
                return Task.FromCanceled<Result<string>>(token);
            },
            token));

        unitOfWork.Verify(work => work.RollbackTransactionAsync(), Times.Once);
        unitOfWork.Verify(work => work.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Task<Result<string>> HandleException(
        bool command,
        Func<Task<Result<string>>> next,
        CancellationToken cancellationToken)
        => command
            ? new CommandUnhandledExceptionBehaviour<TestCommand, Result<string>>(
                NullLogger<TestCommand>.Instance, Mock.Of<ICurrentUserService>())
                .Handle(new TestCommand(), () => next(), cancellationToken)
            : new QueryUnhandledExceptionBehaviour<TestQuery, Result<string>>(
                NullLogger<TestQuery>.Instance, Mock.Of<ICurrentUserService>())
                .Handle(new TestQuery(), () => next(), cancellationToken);

    private sealed class TestCommand : ICommand<Result<string>>;

    private sealed class TestQuery : IQuery<Result<string>>;

    private sealed class AuditableQuery : IAuditableRequest<Result<string>>
    {
        public string Identifier() => "participant";
    }

    private sealed class TokenCheckingSpecification(CancellationToken expectedToken) : AccessValidationSpecification
    {
        public override int Order => 0;

        public bool WasCalled { get; private set; }

        protected override Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken)
        {
            cancellationToken.ShouldBe(expectedToken);
            WasCalled = true;
            return Task.FromResult(AccessGrant.Granted);
        }
    }
}
