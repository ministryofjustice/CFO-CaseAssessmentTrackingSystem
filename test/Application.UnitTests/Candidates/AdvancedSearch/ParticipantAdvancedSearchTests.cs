using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Candidates.AdvancedSearch;

public class ParticipantAdvancedSearchTests
{
    [TestCase("A1234BC")]
    [TestCase("AB1234567")]
    public void Validator_WithValidIdentifierLength_ShouldPass(string identifier)
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(identifier: identifier));

        result.IsValid.ShouldBeTrue();
    }

    [TestCase("A")]
    [TestCase("AB123456")]
    [TestCase("AB12345678")]
    public void Validator_WithInvalidIdentifierLength_ShouldFail(string identifier)
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(identifier: identifier));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x =>
            x.PropertyName == nameof(ParticipantAdvancedSearchQuery.Identifier)
            && x.ErrorMessage == "Identifier must be a 7 character external identifier or a 9 character CATS Id");
    }

    [Test]
    public void Validator_WithWhitespaceIdentifier_ShouldFail()
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(identifier: " AB1234567 "));

        result.IsValid.ShouldBeFalse();
    }

    [TestCase(50, true)]
    [TestCase(51, false)]
    public void Validator_ShouldEnforceLastNameLimit(int length, bool isValid)
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(
            lastName: new string('A', length),
            dateOfBirth: DateTime.Today.AddYears(-20)));

        result.IsValid.ShouldBe(isValid);
        if (!isValid)
        {
            result.Errors.ShouldContain(x =>
                x.PropertyName == nameof(ParticipantAdvancedSearchQuery.LastName)
                && x.ErrorMessage == "Last name must be 50 characters or fewer");
        }
    }

    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("   ", false)]
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
        var result = validator.Validate(CreateValidQuery(
            lastName: lastName,
            dateOfBirth: DateTime.Today.AddYears(-20)));

        result.IsValid.ShouldBe(isValid);
        if (!isValid)
        {
            result.Errors.ShouldContain(x => x.PropertyName == nameof(ParticipantAdvancedSearchQuery.LastName));
        }
    }

    [Test]
    public void Validator_WhenDateOfBirthLessThan18YearsAgo_ShouldFail()
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(
            lastName: "Smith",
            dateOfBirth: DateTime.Today.AddYears(-18).AddDays(1)));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x =>
            x.PropertyName == nameof(ParticipantAdvancedSearchQuery.DateOfBirth)
            && x.ErrorMessage == "Must be at least 18 years");
    }

    [Test]
    public void Validator_WhenDateOfBirthAt18YearsAgo_ShouldPass()
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(
            lastName: "Smith",
            dateOfBirth: DateTime.Today.AddYears(-18)));

        result.IsValid.ShouldBeTrue();
    }

    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("   ", false)]
    [TestCase("CallRef123", true)]
    public void Validator_ShouldRequireJustification(string justification, bool isValid)
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(justification: justification));

        result.IsValid.ShouldBe(isValid);
        if (!isValid)
        {
            result.Errors.ShouldContain(x =>
                x.PropertyName == nameof(ParticipantAdvancedSearchQuery.Justification)
                && x.ErrorMessage == "Search justification is required");
        }
    }

    [TestCase(ValidationConstants.PicklistTextLength, true)]
    [TestCase(ValidationConstants.PicklistTextLength + 1, false)]
    public void Validator_ShouldEnforceJustificationLength(int length, bool isValid)
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(justification: new string('A', length)));

        result.IsValid.ShouldBe(isValid);
        if (!isValid)
        {
            result.Errors.ShouldContain(x =>
                x.PropertyName == nameof(ParticipantAdvancedSearchQuery.Justification)
                && x.ErrorMessage == $"Search justification must be less than or equal to {ValidationConstants.PicklistTextLength} characters");
        }
    }

    [Test]
    public void Validator_WhenDateOfBirthProvidedWithoutLastName_ShouldFail()
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(lastName: "", dateOfBirth: DateTime.Today.AddYears(-20)));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x =>
            x.PropertyName == nameof(ParticipantAdvancedSearchQuery.LastName)
            && x.ErrorMessage == "Last name is required when date of birth is provided");
    }

    [Test]
    public void Validator_WhenLastNameProvidedWithoutDateOfBirth_ShouldFail()
    {
        var validator = new ParticipantAdvancedSearchQueryValidator();
        var result = validator.Validate(CreateValidQuery(lastName: "Smith", dateOfBirth: null));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x =>
            x.PropertyName == nameof(ParticipantAdvancedSearchQuery.DateOfBirth)
            && x.ErrorMessage == "Date of birth is required when last name is provided");
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

    private static ParticipantAdvancedSearchQuery CreateValidQuery(
        string identifier = "AB1234567",
        string lastName = "",
        DateTime? dateOfBirth = null,
        string justification = "CallRef123")
        => new()
        {
            Identifier = identifier,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Justification = justification,
            CurrentUser = new UserProfile
            {
                UserName = "test.user",
                Email = "test@example.com",
                UserId = "test-user"
            }
        };
}
