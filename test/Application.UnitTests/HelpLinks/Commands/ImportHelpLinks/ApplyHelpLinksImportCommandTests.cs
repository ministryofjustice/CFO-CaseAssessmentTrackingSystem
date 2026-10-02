#nullable enable
using Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.HelpLinks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HelpLinks.Commands.ImportHelpLinks;

public class ApplyHelpLinksImportCommandTests
{
    private Mock<IHelpLinkRepository> _repository = null!;
    private Mock<IHelpLinkCounter> _helpLinkCounter = null!;
    private ApplyHelpLinksImportCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IHelpLinkRepository>();
        _helpLinkCounter = new Mock<IHelpLinkCounter>();
        _helpLinkCounter.Setup(c => c.CountForPageAndTab(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .Returns(0);
        _handler = new ApplyHelpLinksImportCommandHandler(_repository.Object, _helpLinkCounter.Object);
    }

    private static HelpLinkImportDecisionDto NewEntryDecision() => new()
    {
        Title = "New Title",
        Description = "New Description",
        PageKey = "/pages/workspace/participants/{id}",
        TabName = "About",
        Urls = [new HelpLinkUrlDto { Url = "https://example.com/new" }]
    };

    private static HelpLink CreateExistingHelpLink() => HelpLink.Create(
        "Original Title",
        "Original Description",
        [new HelpLinkUrlInput("https://example.com/original", null)],
        "/pages/workspace/participants/{id}",
        "Inductions",
        Mock.Of<IHelpLinkCounter>());

    [Test]
    public async Task Handle_WithNewEntryDecision_ShouldCreateHelpLinkAndCountAsCreated()
    {
        HelpLink? added = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<HelpLink>()))
            .Callback<HelpLink>(h => added = h)
            .Returns(Task.CompletedTask);

        var command = new ApplyHelpLinksImportCommand { Decisions = [NewEntryDecision()] };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Created.ShouldBe(1);
        result.Data!.Updated.ShouldBe(0);
        added.ShouldNotBeNull();
        added.Title.ShouldBe("New Title");
        _repository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithExistingHelpLinkIdDecision_ShouldUpdateHelpLinkAndCountAsUpdated()
    {
        var existing = CreateExistingHelpLink();
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);

        var decision = new HelpLinkImportDecisionDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            PageKey = existing.PageKey,
            TabName = existing.TabName,
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/updated" }],
            ExistingHelpLinkId = existing.Id
        };

        var command = new ApplyHelpLinksImportCommand { Decisions = [decision] };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Created.ShouldBe(0);
        result.Data!.Updated.ShouldBe(1);
        existing.Title.ShouldBe("Updated Title");
        existing.Urls.Single().Url.ShouldBe("https://example.com/updated");
        _repository.Verify(r => r.AddAsync(It.IsAny<HelpLink>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithMixOfNewAndUpdateDecisions_ShouldApplyBothAndCountCorrectly()
    {
        var existing = CreateExistingHelpLink();
        _repository.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repository.Setup(r => r.AddAsync(It.IsAny<HelpLink>())).Returns(Task.CompletedTask);

        var updateDecision = new HelpLinkImportDecisionDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            PageKey = existing.PageKey,
            TabName = existing.TabName,
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/updated" }],
            ExistingHelpLinkId = existing.Id
        };

        var command = new ApplyHelpLinksImportCommand { Decisions = [NewEntryDecision(), updateDecision] };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Data!.Created.ShouldBe(1);
        result.Data!.Updated.ShouldBe(1);
    }

    [Test]
    public async Task Handle_WithNewEntryDecision_WhenAlreadyExistsInDatabase_ShouldSkipAndNotCreateDuplicate()
    {
        _helpLinkCounter.Setup(c => c.CountForPageAndTab(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .Returns(1);

        var command = new ApplyHelpLinksImportCommand { Decisions = [NewEntryDecision()] };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Created.ShouldBe(0);
        result.Data!.Skipped.ShouldBe(1);
        _repository.Verify(r => r.AddAsync(It.IsAny<HelpLink>()), Times.Never);
    }

    [Test]
    public async Task Handle_WithDuplicateNewEntryDecisionsInSameBatch_ShouldOnlyCreateFirstAndSkipRest()
    {
        _repository.Setup(r => r.AddAsync(It.IsAny<HelpLink>())).Returns(Task.CompletedTask);

        var command = new ApplyHelpLinksImportCommand { Decisions = [NewEntryDecision(), NewEntryDecision()] };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data!.Created.ShouldBe(1);
        result.Data!.Skipped.ShouldBe(1);
        _repository.Verify(r => r.AddAsync(It.IsAny<HelpLink>()), Times.Once);
    }

    [Test]
    public void Validator_WithNoDecisions_ShouldFail()
    {
        var validator = new ApplyHelpLinksImportCommandValidator();
        var command = new ApplyHelpLinksImportCommand { Decisions = [] };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public void Validator_WithInvalidDecision_ShouldFail()
    {
        var validator = new ApplyHelpLinksImportCommandValidator();
        var decision = NewEntryDecision();
        decision.Title = "";

        var command = new ApplyHelpLinksImportCommand { Decisions = [decision] };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Decisions[0].Title");
    }

    [Test]
    public void Validator_WithValidDecisions_ShouldPass()
    {
        var validator = new ApplyHelpLinksImportCommandValidator();
        var command = new ApplyHelpLinksImportCommand { Decisions = [NewEntryDecision()] };

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }
}
