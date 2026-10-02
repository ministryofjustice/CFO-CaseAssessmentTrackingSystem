#nullable enable
using Cfo.Cats.Application.Features.HelpLinks.Commands;
using Cfo.Cats.Application.Features.HelpLinks.Commands.EditHelpLink;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.HelpLinks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HelpLinks.Commands;

public class EditHelpLinkCommandTests
{
    private Mock<IHelpLinkRepository> _repository = null!;
    private Mock<IHelpLinkCounter> _helpLinkCounter = null!;
    private EditHelpLinkCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IHelpLinkRepository>();
        _helpLinkCounter = new Mock<IHelpLinkCounter>();
        _helpLinkCounter.Setup(c => c.CountForPageAndTab(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .Returns(0);
        _handler = new EditHelpLinkCommandHandler(_repository.Object, _helpLinkCounter.Object);
    }

    private static HelpLink CreateHelpLink() => HelpLink.Create(
        "Original Title",
        "Original Description",
        [new HelpLinkUrlInput("https://example.com/original", null)],
        "/pages/workspace/participants/{id}",
        "Inductions",
        Mock.Of<IHelpLinkCounter>());

    private static EditHelpLinkCommand ValidCommand(Guid id) => new()
    {
        HelpLinkId = id,
        NewTitle = "Updated Title",
        NewDescription = "Updated Description",
        NewUrls = [new HelpLinkUrlDto { Url = "https://example.com/updated" }],
        NewPageKey = "/pages/workspace/other",
        NewTabName = "Status History"
    };

    [Test]
    public async Task Handle_WithValidCommand_ShouldUpdateHelpLink()
    {
        var helpLink = CreateHelpLink();
        _repository.Setup(r => r.GetByIdAsync(helpLink.Id)).ReturnsAsync(helpLink);

        var command = ValidCommand(helpLink.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        helpLink.Title.ShouldBe("Updated Title");
        helpLink.Description.ShouldBe("Updated Description");
        helpLink.Urls.Single().Url.ShouldBe("https://example.com/updated");
        helpLink.PageKey.ShouldBe("/pages/workspace/other");
        helpLink.TabName.ShouldBe("Status History");
    }

    [Test]
    public async Task Handle_WithMultipleNewUrls_ShouldUpdateAllUrls()
    {
        var helpLink = CreateHelpLink();
        _repository.Setup(r => r.GetByIdAsync(helpLink.Id)).ReturnsAsync(helpLink);

        var command = ValidCommand(helpLink.Id);
        command.NewUrls =
        [
            new HelpLinkUrlDto { Url = "https://example.com/guidance", DisplayName = "Guidance" },
            new HelpLinkUrlDto { Url = "https://example.com/slides", DisplayName = "PowerPoint" }
        ];

        await _handler.Handle(command, CancellationToken.None);

        helpLink.Urls.Count.ShouldBe(2);
        helpLink.Urls.ShouldContain(u => u.Url == "https://example.com/guidance" && u.DisplayName == "Guidance");
        helpLink.Urls.ShouldContain(u => u.Url == "https://example.com/slides" && u.DisplayName == "PowerPoint");
    }

    [Test]
    public async Task Handle_ShouldFetchByHelpLinkId()
    {
        var helpLink = CreateHelpLink();
        _repository.Setup(r => r.GetByIdAsync(helpLink.Id)).ReturnsAsync(helpLink);

        await _handler.Handle(ValidCommand(helpLink.Id), CancellationToken.None);

        _repository.Verify(r => r.GetByIdAsync(helpLink.Id), Times.Once);
    }

    [Test]
    public void Handle_WithDuplicatePageAndTab_ShouldThrowBusinessRuleException()
    {
        var helpLink = CreateHelpLink();
        _repository.Setup(r => r.GetByIdAsync(helpLink.Id)).ReturnsAsync(helpLink);
        _helpLinkCounter.Setup(c => c.CountForPageAndTab(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .Returns(1);

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(ValidCommand(helpLink.Id), CancellationToken.None));
    }

    [Test]
    public void Validator_WithEmptyHelpLinkId_ShouldFail()
    {
        var validator = new EditHelpLinkCommandValidator();
        var command = ValidCommand(Guid.Empty);

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "HelpLinkId");
    }

    [Test]
    public void Validator_WithEmptyNewTitle_ShouldFail()
    {
        var validator = new EditHelpLinkCommandValidator();
        var command = ValidCommand(Guid.NewGuid());
        command.NewTitle = "";

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewTitle");
    }

    [Test]
    public void Validator_WithNoNewUrls_ShouldFail()
    {
        var validator = new EditHelpLinkCommandValidator();
        var command = ValidCommand(Guid.NewGuid());
        command.NewUrls = [];

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewUrls");
    }

    [Test]
    public void Validator_WithNonAbsoluteUrl_ShouldFail()
    {
        var validator = new EditHelpLinkCommandValidator();
        var command = ValidCommand(Guid.NewGuid());
        command.NewUrls = [new HelpLinkUrlDto { Url = "not-a-url" }];

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewUrls[0].Url");
    }

    [Test]
    public void Validator_WithDisplayNameTooLong_ShouldFail()
    {
        var validator = new EditHelpLinkCommandValidator();
        var command = ValidCommand(Guid.NewGuid());
        command.NewUrls = [new HelpLinkUrlDto { Url = "https://example.com", DisplayName = new string('A', HelpLinkConstants.DisplayNameMaximumLength + 1) }];

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewUrls[0].DisplayName");
    }

    [Test]
    public void Validator_WithTabNameTooLong_ShouldFail()
    {
        var validator = new EditHelpLinkCommandValidator();
        var command = ValidCommand(Guid.NewGuid());
        command.NewTabName = new string('A', HelpLinkConstants.TabNameMaximumLength + 1);

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "NewTabName");
    }

    [Test]
    public void Validator_WithValidCommand_ShouldPass()
    {
        var validator = new EditHelpLinkCommandValidator();

        var result = validator.Validate(ValidCommand(Guid.NewGuid()));

        result.IsValid.ShouldBeTrue();
    }
}
