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

    [TestCase(50, true)]
    [TestCase(51, false)]
    public void Validator_ShouldEnforceLastNameLimit(int length, bool isValid)
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(new ParticipantAdvancedSearchQuery
        {
            Identifier = "AB1234567",
            LastName = new string('A', length),
            CurrentUser = null!
        });

        result.IsValid.ShouldBe(isValid);
        if (!isValid)
        {
            var error = result.Errors.ShouldHaveSingleItem();
            error.PropertyName.ShouldBe(nameof(ParticipantAdvancedSearchQuery.LastName));
            error.ErrorMessage.ShouldBe("Last name must be 50 characters or fewer");
        }
    }

    [TestCase(null, true)]
    [TestCase("", true)]
    [TestCase("   ", true)]
    [TestCase("Smith", true)]
    [TestCase("O'Connor", true)]
    [TestCase("O’Connor", true)]
    [TestCase("Smith-Jones", true)]
    [TestCase("De la Cruz", true)]
    [TestCase("García", true)]
    [TestCase("Smith&Jones", false)]
    [TestCase("Smith#Jones", false)]
    [TestCase("Smith+Jones", false)]
    [TestCase("Smith123", false)]
    public void Validator_ShouldEnforceDmsSurnameFormat(string lastName, bool isValid)
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(new ParticipantAdvancedSearchQuery
        {
            Identifier = "AB1234567",
            LastName = lastName,
            CurrentUser = null!
        });

        result.IsValid.ShouldBe(isValid);
        if (!isValid)
        {
            result.Errors.ShouldHaveSingleItem().PropertyName.ShouldBe(nameof(ParticipantAdvancedSearchQuery.LastName));
        }
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
