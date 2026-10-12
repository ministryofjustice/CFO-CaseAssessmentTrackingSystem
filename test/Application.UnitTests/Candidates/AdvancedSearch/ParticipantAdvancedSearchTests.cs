using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Application.Common.Interfaces;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Candidates.AdvancedSearch;

public class ParticipantAdvancedSearchTests
{
    [Test]
    public void Validator_WithValidIdentifier_ShouldPass()
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(new ParticipantAdvancedSearchQuery
        {
            Identifier = "AB1234567",
            CurrentUser = null!
        });

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_WithWhitespaceIdentifier_ShouldFail()
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(new ParticipantAdvancedSearchQuery
        {
            Identifier = " AB1234567 ",
            CurrentUser = null!
        });

        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public void SelectDmsUpcis_ShouldRemoveDuplicatesAndCatsMatches()
    {
        var selected = ParticipantAdvancedSearchMerge.SelectDmsUpcis(
        [
            new SearchResult("DMS-2", 2),
            new SearchResult("CATS-1", 1),
            new SearchResult("DMS-2", 3)
        ],
        new HashSet<string> { "CATS-1" },
        "CRN1234",
        maximumResults: 10);

        selected.ShouldBe(["DMS-2"]);
    }

    [Test]
    public void SelectDmsUpcis_WhenNoDmsMatchAndIdentifierIsCatsId_ShouldUseIdentifierFallback()
    {
        var selected = ParticipantAdvancedSearchMerge.SelectDmsUpcis(
            [],
            new HashSet<string>(),
            "AB1234567",
            maximumResults: 10);

        selected.ShouldBe(["AB1234567"]);
    }

    [Test]
    public void ResultWithDmsWarning_ShouldExposeWarning()
    {
        var result = new ParticipantAdvancedSearchResult
        {
            Results = [],
            Warning = "DMS search could not be completed."
        };

        result.HasWarning.ShouldBeTrue();
        result.Warning.ShouldBe("DMS search could not be completed.");
    }
}
