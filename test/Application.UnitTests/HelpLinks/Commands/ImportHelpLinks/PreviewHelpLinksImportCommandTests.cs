#nullable enable
using System.Text;
using Cfo.Cats.Application.Features.HelpLinks.Commands.ImportHelpLinks;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.HelpLinks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HelpLinks.Commands.ImportHelpLinks;

public class PreviewHelpLinksImportCommandTests
{
    private Mock<IHelpLinkRepository> _repository = null!;
    private PreviewHelpLinksImportCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IHelpLinkRepository>();
        _handler = new PreviewHelpLinksImportCommandHandler(_repository.Object);
    }

    private static HelpLink CreateExistingHelpLink() => HelpLink.Create(
        "Existing Title",
        "Existing Description",
        [new HelpLinkUrlInput("https://example.com/existing", null)],
        "/pages/workspace/participants/{id}",
        "Inductions",
        Mock.Of<IHelpLinkCounter>());

    private static byte[] ToExportBytes(params HelpLinkExportDto[] items) =>
        Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(items));

    [Test]
    public async Task Handle_WithBrandNewEntry_ShouldReturnNonConflictItem()
    {
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<HelpLink>)Array.Empty<HelpLink>());

        var incoming = new HelpLinkExportDto
        {
            Title = "New Title",
            Description = "New Description",
            PageKey = "/pages/workspace/participants/{id}",
            TabName = "About",
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/new" }]
        };

        var command = new PreviewHelpLinksImportCommand(ToExportBytes(incoming));
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        var item = result.Data!.Single();
        item.IsConflict.ShouldBeFalse();
        item.Existing.ShouldBeNull();
        item.ExistingHelpLinkId.ShouldBeNull();
        item.Incoming.Title.ShouldBe("New Title");
    }

    [Test]
    public async Task Handle_WithMatchingPageKeyAndTabName_ShouldReturnConflictItemWithBothVersions()
    {
        var existing = CreateExistingHelpLink();
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<HelpLink>)[existing]);

        var incoming = new HelpLinkExportDto
        {
            Title = "Imported Title",
            Description = "Imported Description",
            PageKey = existing.PageKey,
            TabName = existing.TabName,
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/imported" }]
        };

        var command = new PreviewHelpLinksImportCommand(ToExportBytes(incoming));
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var item = result.Data!.Single();
        item.IsConflict.ShouldBeTrue();
        item.ExistingHelpLinkId.ShouldBe(existing.Id);
        item.Existing.ShouldNotBeNull();
        item.Existing!.Title.ShouldBe("Existing Title");
        item.Incoming.Title.ShouldBe("Imported Title");
    }

    [Test]
    public async Task Handle_WithDifferentTabNameOnSamePage_ShouldNotBeTreatedAsConflict()
    {
        var existing = CreateExistingHelpLink();
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<HelpLink>)[existing]);

        var incoming = new HelpLinkExportDto
        {
            Title = "Imported Title",
            Description = "Imported Description",
            PageKey = existing.PageKey,
            TabName = "About", // different tab to the existing "Inductions" entry
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/imported" }]
        };

        var command = new PreviewHelpLinksImportCommand(ToExportBytes(incoming));
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Data!.Single().IsConflict.ShouldBeFalse();
    }

    [Test]
    public async Task Handle_WithMixOfNewAndConflicting_ShouldReturnBothCorrectlyFlagged()
    {
        var existing = CreateExistingHelpLink();
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<HelpLink>)[existing]);

        var conflicting = new HelpLinkExportDto
        {
            Title = "Imported Title",
            Description = "Imported Description",
            PageKey = existing.PageKey,
            TabName = existing.TabName,
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/imported" }]
        };

        var brandNew = new HelpLinkExportDto
        {
            Title = "New Title",
            Description = "New Description",
            PageKey = "/pages/workspace/other",
            TabName = null,
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/new" }]
        };

        var command = new PreviewHelpLinksImportCommand(ToExportBytes(conflicting, brandNew));
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Data!.Length.ShouldBe(2);
        result.Data!.Count(x => x.IsConflict).ShouldBe(1);
        result.Data!.Count(x => x.IsConflict == false).ShouldBe(1);
    }

    [Test]
    public async Task Handle_WithInvalidJson_ShouldReturnFailure()
    {
        var command = new PreviewHelpLinksImportCommand(Encoding.UTF8.GetBytes("not valid json"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
    }

    [Test]
    public async Task Handle_WithEmptyArray_ShouldReturnFailure()
    {
        var command = new PreviewHelpLinksImportCommand(ToExportBytes());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeFalse();
    }

    [Test]
    public async Task Handle_WithMatchingContent_ShouldBeUnchanged()
    {
        var existing = CreateExistingHelpLink();
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<HelpLink>)[existing]);

        var incoming = new HelpLinkExportDto
        {
            Title = existing.Title,
            Description = existing.Description,
            PageKey = existing.PageKey,
            TabName = existing.TabName,
            Urls = existing.Urls.Select(u => new HelpLinkUrlDto { Url = u.Url, DisplayName = u.DisplayName }).ToList()
        };

        var command = new PreviewHelpLinksImportCommand(ToExportBytes(incoming));
        var result = await _handler.Handle(command, CancellationToken.None);

        var item = result.Data!.Single();
        item.Status.ShouldBe(HelpLinkImportMatchStatus.Unchanged);
        item.IsConflict.ShouldBeFalse();
        item.IsUnchanged.ShouldBeTrue();
    }

    [Test]
    public async Task Handle_WithDuplicateExistingDataForSamePageAndTab_ShouldNotThrowAndShouldUseNewestAsCanonical()
    {
        var older = HelpLink.Create(
            "Older Title",
            "Older Description",
            [new HelpLinkUrlInput("https://example.com/older", null)],
            "/pages/workspace/participants/{id}",
            "Inductions",
            Mock.Of<IHelpLinkCounter>());

        var newer = HelpLink.Create(
            "Newer Title",
            "Newer Description",
            [new HelpLinkUrlInput("https://example.com/newer", null)],
            "/pages/workspace/participants/{id}",
            "Inductions",
            Mock.Of<IHelpLinkCounter>());

        older.Created = DateTime.UtcNow.AddDays(-1);
        newer.Created = DateTime.UtcNow;

        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync((IReadOnlyList<HelpLink>)[older, newer]);

        var incoming = new HelpLinkExportDto
        {
            Title = "Imported Title",
            Description = "Imported Description",
            PageKey = "/pages/workspace/participants/{id}",
            TabName = "Inductions",
            Urls = [new HelpLinkUrlDto { Url = "https://example.com/imported" }]
        };

        var command = new PreviewHelpLinksImportCommand(ToExportBytes(incoming));

        // Should not throw despite two existing HelpLinks sharing the same Page/Tab.
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.ShouldBeTrue();
        var item = result.Data!.Single();
        item.ExistingHelpLinkId.ShouldBe(newer.Id);
        item.Existing!.Title.ShouldBe("Newer Title");
    }

    [Test]
    public void Validator_WithEmptyData_ShouldFail()
    {
        var validator = new PreviewHelpLinksImportCommandValidator();
        var command = new PreviewHelpLinksImportCommand([]);

        var result = validator.Validate(command);

        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public void Validator_WithData_ShouldPass()
    {
        var validator = new PreviewHelpLinksImportCommandValidator();
        var command = new PreviewHelpLinksImportCommand([1, 2, 3]);

        var result = validator.Validate(command);

        result.IsValid.ShouldBeTrue();
    }
}
