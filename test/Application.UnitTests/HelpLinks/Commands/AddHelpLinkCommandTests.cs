#nullable enable
using Cfo.Cats.Application.Features.HelpLinks.Commands;
using Cfo.Cats.Application.Features.HelpLinks.Commands.AddHelpLink;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.HelpLinks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HelpLinks.Commands;

public class AddHelpLinkCommandTests
{
    private Mock<IHelpLinkRepository> _repository = null!;
    private Mock<IHelpLinkCounter> _helpLinkCounter = null!;
    private AddHelpLinkCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IHelpLinkRepository>();
        _helpLinkCounter = new Mock<IHelpLinkCounter>();
        _helpLinkCounter.Setup(c => c.CountForPageAndTab(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .Returns(0);
        _handler = new AddHelpLinkCommandHandler(_repository.Object, _helpLinkCounter.Object);
    }

    private static AddHelpLinkCommand ValidCommand() => new()
    {
        Title = "Test Title",
        Description = "Test Description",
        Urls = [new HelpLinkUrlDto { Url = "https://example.com/help" }],
        PageKey = "/pages/workspace/participants/{id}",
        TabName = "Inductions"
    };

    [Test]
    public async Task Handle_WithValidCommand_ShouldCreateHelpLink()
    {
        HelpLink? added = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<HelpLink>()))
            .Callback<HelpLink>(h => added = h)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        added.ShouldNotBeNull();
        added.Title.ShouldBe("Test Title");
        added.PageKey.ShouldBe("/pages/workspace/participants/{id}");
        added.TabName.ShouldBe("Inductions");
    }

    [Test]
    public async Task Handle_WithMultipleUrls_ShouldCreateHelpLinkWithAllUrls()
    {
        HelpLink? added = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<HelpLink>()))
            .Callback<HelpLink>(h => added = h)
            .Returns(Task.CompletedTask);

        var command = ValidCommand();
        command.Urls =
        [
            new HelpLinkUrlDto { Url = "https://example.com/guidance", DisplayName = "Guidance" },
            new HelpLinkUrlDto { Url = "https://example.com/slides", DisplayName = "PowerPoint" }
        ];

        await _handler.Handle(command, CancellationToken.None);

        added.ShouldNotBeNull();
        added.Urls.Count.ShouldBe(2);
        added.Urls.ShouldContain(u => u.Url == "https://example.com/guidance" && u.DisplayName == "Guidance");
        added.Urls.ShouldContain(u => u.Url == "https://example.com/slides" && u.DisplayName == "PowerPoint");
    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldCallRepositoryAdd()
    {
        _repository.Setup(r => r.AddAsync(It.IsAny<HelpLink>())).Returns(Task.CompletedTask);

        await _handler.Handle(ValidCommand(), CancellationToken.None);

        _repository.Verify(r => r.AddAsync(It.IsAny<HelpLink>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithNoTabName_ShouldCreatePageLevelHelpLink()
    {
        HelpLink? added = null;
        _repository.Setup(r => r.AddAsync(It.IsAny<HelpLink>()))
            .Callback<HelpLink>(h => added = h)
            .Returns(Task.CompletedTask);

        var command = ValidCommand();
        command.TabName = null;

        await _handler.Handle(command, CancellationToken.None);

        added.ShouldNotBeNull();
        added.TabName.ShouldBeNull();
    }

    [Test]
    public void Handle_WithDuplicatePageAndTab_ShouldThrowBusinessRuleException()
    {
        _helpLinkCounter.Setup(c => c.CountForPageAndTab(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .Returns(1);

        Should.Throw<BusinessRuleValidationException>(async () =>
            await _handler.Handle(ValidCommand(), CancellationToken.None));
    }

    [Test]
    public void Validator_WithEmptyTitle_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.Title = "";

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Title");
    }

    [Test]
    public void Validator_WithTitleTooLong_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.Title = new string('A', HelpLinkConstants.TitleMaximumLength + 1);

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Title");
    }

    [Test]
    public void Validator_WithEmptyDescription_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.Description = "";

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
    }

    [Test]
    public void Validator_WithNoUrls_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.Urls = [];

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Urls");
    }

    [Test]
    public void Validator_WithEmptyUrl_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.Urls = [new HelpLinkUrlDto { Url = "" }];

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Urls[0].Url");
    }

    [Test]
    public void Validator_WithNonAbsoluteUrl_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.Urls = [new HelpLinkUrlDto { Url = "/relative/path" }];

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Urls[0].Url");
    }

    [Test]
    public void Validator_WithDisplayNameTooLong_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.Urls = [new HelpLinkUrlDto { Url = "https://example.com", DisplayName = new string('A', HelpLinkConstants.DisplayNameMaximumLength + 1) }];

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Urls[0].DisplayName");
    }

    [Test]
    public void Validator_WithEmptyPageKey_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.PageKey = "";

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageKey");
    }

    [Test]
    public void Validator_WithTabNameTooLong_ShouldFail()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.TabName = new string('A', HelpLinkConstants.TabNameMaximumLength + 1);

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "TabName");
    }

    [Test]
    public void Validator_WithNullTabName_ShouldPass()
    {
        var validator = new AddHelpLinkCommandValidator();
        var command = ValidCommand();
        command.TabName = null;

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithValidCommand_ShouldPass()
    {
        var validator = new AddHelpLinkCommandValidator();

        var result = validator.Validate(ValidCommand());

        result.IsValid.ShouldBeTrue();
    }
}
