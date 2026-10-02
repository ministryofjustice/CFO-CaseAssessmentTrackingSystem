#nullable enable
using System.Linq;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.HelpLinks;
using Cfo.Cats.Domain.HelpLinks.Events;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.HelpLinks;

public class HelpLinkTests
{
    private TestHelpLinkCounter _helpLinkCounter = null!;

    [SetUp]
    public void Setup() => _helpLinkCounter = new TestHelpLinkCounter();

    private static HelpLinkUrlInput[] ValidUrls(string url = "https://example.com/help", string? displayName = null) =>
        [new HelpLinkUrlInput(url, displayName)];

    [Test]
    public void Create_WithValidData_ShouldSucceed()
    {
        var helpLink = HelpLink.Create(
            "Test Title",
            "Test Description",
            ValidUrls(),
            "/pages/workspace/participants/{id}",
            "Inductions",
            _helpLinkCounter);

        helpLink.ShouldNotBeNull();
        helpLink.Title.ShouldBe("Test Title");
        helpLink.Description.ShouldBe("Test Description");
        helpLink.Urls.Single().Url.ShouldBe("https://example.com/help");
        helpLink.PageKey.ShouldBe("/pages/workspace/participants/{id}");
        helpLink.TabName.ShouldBe("Inductions");
    }

    [Test]
    public void Create_WithMultipleUrls_ShouldStoreAllUrls()
    {
        var helpLink = HelpLink.Create(
            "Test Title",
            "Test Description",
            [
                new HelpLinkUrlInput("https://example.com/guidance", "Guidance"),
                new HelpLinkUrlInput("https://example.com/slides", "PowerPoint")
            ],
            "/pages/workspace/participants/{id}",
            null,
            _helpLinkCounter);

        helpLink.Urls.Count.ShouldBe(2);
        helpLink.Urls.ShouldContain(u => u.Url == "https://example.com/guidance" && u.DisplayName == "Guidance");
        helpLink.Urls.ShouldContain(u => u.Url == "https://example.com/slides" && u.DisplayName == "PowerPoint");
    }

    [Test]
    public void Create_WithUrlWithoutDisplayName_ShouldExposeUrlAsLabel()
    {
        var helpLink = HelpLink.Create(
            "Test Title",
            "Test Description",
            ValidUrls("https://example.com/help"),
            "/pages/workspace/participants/{id}",
            null,
            _helpLinkCounter);

        helpLink.Urls.Single().Label.ShouldBe("https://example.com/help");
    }

    [Test]
    public void Create_WithUrlWithDisplayName_ShouldExposeDisplayNameAsLabel()
    {
        var helpLink = HelpLink.Create(
            "Test Title",
            "Test Description",
            ValidUrls("https://example.com/help", "Guidance"),
            "/pages/workspace/participants/{id}",
            null,
            _helpLinkCounter);

        helpLink.Urls.Single().Label.ShouldBe("Guidance");
    }

    [Test]
    public void Create_WithNullTabName_ShouldSucceedWithNullTabName()
    {
        var helpLink = HelpLink.Create(
            "Test Title",
            "Test Description",
            ValidUrls(),
            "/pages/workspace/participants/{id}",
            null,
            _helpLinkCounter);

        helpLink.TabName.ShouldBeNull();
    }

    [Test]
    public void Create_WithWhitespaceTabName_ShouldNormalizeToNull()
    {
        var helpLink = HelpLink.Create(
            "Test Title",
            "Test Description",
            ValidUrls(),
            "/pages/workspace/participants/{id}",
            "   ",
            _helpLinkCounter);

        helpLink.TabName.ShouldBeNull();
    }

    [Test]
    public void Create_WithNullTitle_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create(null!, "Description", ValidUrls(), "/page", null, _helpLinkCounter))
            .Message.ShouldContain("Help Link Title cannot be null or empty.");

    [Test]
    public void Create_WithEmptyDescription_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "", ValidUrls(), "/page", null, _helpLinkCounter))
            .Message.ShouldContain("Help Link Description cannot be null or empty.");

    [Test]
    public void Create_WithNoUrls_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", [], "/page", null, _helpLinkCounter))
            .Message.ShouldContain("At least one Help Link Url must be provided.");

    [Test]
    public void Create_WithOnlyBlankUrls_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", [new HelpLinkUrlInput("", null)], "/page", null, _helpLinkCounter))
            .Message.ShouldContain("At least one Help Link Url must be provided.");

    [Test]
    public void Create_WithEmptyPageKey_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", ValidUrls(), "", null, _helpLinkCounter))
            .Message.ShouldContain("Help Link Page cannot be null or empty.");

    [Test]
    public void Create_WithTitleTooLong_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create(new string('A', HelpLinkConstants.TitleMaximumLength + 1), "Description", ValidUrls(), "/page", null, _helpLinkCounter))
            .Message.ShouldContain($"Help Link Title cannot exceed {HelpLinkConstants.TitleMaximumLength} characters.");

    [Test]
    public void Create_WithDescriptionTooLong_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", new string('A', HelpLinkConstants.DescriptionMaximumLength + 1), ValidUrls(), "/page", null, _helpLinkCounter))
            .Message.ShouldContain($"Help Link Description cannot exceed {HelpLinkConstants.DescriptionMaximumLength} characters.");

    [Test]
    public void Create_WithUrlTooLong_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", ValidUrls("https://example.com/" + new string('A', HelpLinkConstants.UrlMaximumLength)), "/page", null, _helpLinkCounter))
            .Message.ShouldContain($"Help Link Url cannot exceed {HelpLinkConstants.UrlMaximumLength} characters.");

    [Test]
    public void Create_WithDisplayNameTooLong_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", ValidUrls(displayName: new string('A', HelpLinkConstants.DisplayNameMaximumLength + 1)), "/page", null, _helpLinkCounter))
            .Message.ShouldContain($"Help Link Url display name cannot exceed {HelpLinkConstants.DisplayNameMaximumLength} characters.");

    [Test]
    public void Create_WithPageKeyTooLong_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", ValidUrls(), new string('A', HelpLinkConstants.PageKeyMaximumLength + 1), null, _helpLinkCounter))
            .Message.ShouldContain($"Help Link Page cannot exceed {HelpLinkConstants.PageKeyMaximumLength} characters.");

    [Test]
    public void Create_WithTabNameTooLong_ShouldThrowBusinessRuleException() =>
        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", ValidUrls(), "/page", new string('A', HelpLinkConstants.TabNameMaximumLength + 1), _helpLinkCounter))
            .Message.ShouldContain($"Help Link Tab Name cannot exceed {HelpLinkConstants.TabNameMaximumLength} characters.");

    [Test]
    public void Create_WithExistingPageAndTab_ShouldThrowBusinessRuleException()
    {
        _helpLinkCounter.SetCount(1);

        Should.Throw<BusinessRuleValidationException>(() =>
                HelpLink.Create("Title", "Description", ValidUrls(), "/pages/workspace/participants/{id}", "Inductions", _helpLinkCounter))
            .Message.ShouldContain("A help link already exists for this page and tab.");
    }

    [Test]
    public void Edit_WithValidChanges_ShouldUpdateAllProperties()
    {
        var helpLink = CreateHelpLink();

        helpLink.Edit(
            "Updated Title",
            "Updated Description",
            ValidUrls("https://example.com/updated"),
            "/pages/workspace/other",
            "Status History",
            _helpLinkCounter);

        helpLink.Title.ShouldBe("Updated Title");
        helpLink.Description.ShouldBe("Updated Description");
        helpLink.Urls.Single().Url.ShouldBe("https://example.com/updated");
        helpLink.PageKey.ShouldBe("/pages/workspace/other");
        helpLink.TabName.ShouldBe("Status History");
    }

    [Test]
    public void Edit_KeepingSameUrlButChangingDisplayName_ShouldRenameInPlace()
    {
        var helpLink = CreateHelpLink();

        helpLink.Edit(
            "Title",
            "Description",
            [new HelpLinkUrlInput("https://example.com", "Renamed")],
            "/pages/workspace/participants/{id}",
            "Inductions",
            _helpLinkCounter);

        helpLink.Urls.Single().Url.ShouldBe("https://example.com");
        helpLink.Urls.Single().DisplayName.ShouldBe("Renamed");
    }

    [Test]
    public void Edit_AddingASecondUrl_ShouldKeepBoth()
    {
        var helpLink = CreateHelpLink();

        helpLink.Edit(
            "Title",
            "Description",
            [
                new HelpLinkUrlInput("https://example.com", null),
                new HelpLinkUrlInput("https://example.com/slides", "PowerPoint")
            ],
            "/pages/workspace/participants/{id}",
            "Inductions",
            _helpLinkCounter);

        helpLink.Urls.Count.ShouldBe(2);
    }

    [Test]
    public void Edit_RemovingAUrl_ShouldLeaveTheRemainder()
    {
        var helpLink = HelpLink.Create(
            "Title",
            "Description",
            [
                new HelpLinkUrlInput("https://example.com/guidance", "Guidance"),
                new HelpLinkUrlInput("https://example.com/slides", "PowerPoint")
            ],
            "/pages/workspace/participants/{id}",
            "Inductions",
            _helpLinkCounter);

        helpLink.Edit(
            "Title",
            "Description",
            [new HelpLinkUrlInput("https://example.com/guidance", "Guidance")],
            "/pages/workspace/participants/{id}",
            "Inductions",
            _helpLinkCounter);

        helpLink.Urls.Single().Url.ShouldBe("https://example.com/guidance");
    }

    [Test]
    public void Edit_WithInvalidTitle_ShouldThrowBusinessRuleException()
    {
        var helpLink = CreateHelpLink();

        Should.Throw<BusinessRuleValidationException>(() =>
                helpLink.Edit("", "Description", ValidUrls(), "/page", null, _helpLinkCounter))
            .Message.ShouldContain("Help Link Title cannot be null or empty.");
    }

    [Test]
    public void Edit_WithNoUrls_ShouldThrowBusinessRuleException()
    {
        var helpLink = CreateHelpLink();

        Should.Throw<BusinessRuleValidationException>(() =>
                helpLink.Edit("Title", "Description", [], "/page", null, _helpLinkCounter))
            .Message.ShouldContain("At least one Help Link Url must be provided.");
    }

    [Test]
    public void Edit_KeepingSamePageAndTab_ShouldNotCollideWithItself()
    {
        var helpLink = CreateHelpLink();

        _helpLinkCounter.SetDuplicateBelongingTo(helpLink.Id);

        Should.NotThrow(() =>
            helpLink.Edit(
                "Updated Title",
                "Description",
                ValidUrls(),
                helpLink.PageKey,
                helpLink.TabName,
                _helpLinkCounter));
    }

    [Test]
    public void Edit_ToAPageAndTabAlreadyUsedByAnotherHelpLink_ShouldThrowBusinessRuleException()
    {
        var helpLink = CreateHelpLink();

        _helpLinkCounter.SetDuplicateBelongingTo(Guid.NewGuid());

        Should.Throw<BusinessRuleValidationException>(() =>
                helpLink.Edit("Title", "Description", ValidUrls(), "/pages/workspace/other", "Status History", _helpLinkCounter))
            .Message.ShouldContain("A help link already exists for this page and tab.");
    }

    [Test]
    public void Delete_ShouldRaiseHelpLinkDeletedDomainEvent()
    {
        var helpLink = CreateHelpLink();

        helpLink.Delete();

        var deleteEvent = helpLink.DomainEvents.OfType<HelpLinkDeletedDomainEvent>().FirstOrDefault();
        deleteEvent.ShouldNotBeNull();
        deleteEvent.Entity.ShouldBe(helpLink);
    }

    private HelpLink CreateHelpLink() => HelpLink.Create(
        "Title",
        "Description",
        [new HelpLinkUrlInput("https://example.com", null)],
        "/pages/workspace/participants/{id}",
        "Inductions",
        _helpLinkCounter);

    private class TestHelpLinkCounter : IHelpLinkCounter
    {
        private int _count;
        private Guid? _matchingId;

        public void SetCount(int count) => _count = count;

        public void SetDuplicateBelongingTo(Guid id)
        {
            _count = 1;
            _matchingId = id;
        }

        public int CountForPageAndTab(string pageKey, string? tabName, Guid? excludeId = null) =>
            _matchingId is not null && excludeId == _matchingId ? 0 : _count;
    }
}
