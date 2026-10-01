#nullable enable
using Cfo.Cats.Application.Features.HelpLinks.Commands;
using Cfo.Cats.Application.Features.HelpLinks.Commands.DeleteHelpLink;
using Cfo.Cats.Domain.HelpLinks;
using Cfo.Cats.Domain.HelpLinks.Events;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HelpLinks.Commands;

public class DeleteHelpLinkCommandTests
{
    private Mock<IHelpLinkRepository> _repository = null!;
    private DeleteHelpLinkCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IHelpLinkRepository>();
        _handler = new DeleteHelpLinkCommandHandler(_repository.Object);
    }

    private static HelpLink CreateHelpLink() => HelpLink.Create(
        "Title",
        "Description",
        [new HelpLinkUrlInput("https://example.com", null)],
        "/pages/workspace/participants/{id}",
        "Inductions",
        Mock.Of<IHelpLinkCounter>());

    [Test]
    public async Task Handle_WithValidCommand_ShouldSucceed()
    {
        var helpLink = CreateHelpLink();
        _repository.Setup(r => r.GetByIdAsync(helpLink.Id)).ReturnsAsync(helpLink);

        var command = new DeleteHelpLinkCommand { HelpLinkId = helpLink.Id };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldRaiseHelpLinkDeletedDomainEvent()
    {
        var helpLink = CreateHelpLink();
        _repository.Setup(r => r.GetByIdAsync(helpLink.Id)).ReturnsAsync(helpLink);

        var command = new DeleteHelpLinkCommand { HelpLinkId = helpLink.Id };

        await _handler.Handle(command, CancellationToken.None);

        var deleteEvent = helpLink.DomainEvents.OfType<HelpLinkDeletedDomainEvent>().FirstOrDefault();
        deleteEvent.ShouldNotBeNull();
        deleteEvent.Entity.ShouldBe(helpLink);
    }

    [Test]
    public async Task Handle_ShouldFetchByHelpLinkId()
    {
        var helpLink = CreateHelpLink();
        _repository.Setup(r => r.GetByIdAsync(helpLink.Id)).ReturnsAsync(helpLink);

        var command = new DeleteHelpLinkCommand { HelpLinkId = helpLink.Id };

        await _handler.Handle(command, CancellationToken.None);

        _repository.Verify(r => r.GetByIdAsync(helpLink.Id), Times.Once);
    }
}
