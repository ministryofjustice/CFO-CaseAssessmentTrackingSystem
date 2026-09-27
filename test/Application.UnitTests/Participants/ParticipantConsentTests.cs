#nullable enable
using Cfo.Cats.Domain.Entities.Participants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants;

public class ParticipantConsentTests
{
    private static Participant CreateParticipant() =>
        Participant.CreateFrom(
            id: "ABC123",
            firstName: "Test",
            middleName: null,
            lastName: "Participant",
            gender: "Male",
            dateOfBirth: new DateTime(1990, 1, 1),
            registrationDetailsJson: null,
            referralSource: "Test",
            referralComments: null,
            locationId: 1,
            nationality: null,
            primaryRecordKeyAtCreation: null,
            ethnicity: null);

    [Test]
    public void AddConsent_WhenFirstConsent_ShouldRemainOpenIndefinitely()
    {
        var participant = CreateParticipant();

        participant.AddConsent(new DateTime(2025, 4, 1), Guid.NewGuid());

        var consent = participant.Consents.Single();
        consent.Lifetime.EndDate.ShouldBe(DateTime.MaxValue.Date);
    }

    [Test]
    public void AddConsent_WhenAddingSubsequentConsent_ShouldCloseThePreviousConsent()
    {
        var participant = CreateParticipant();

        participant.AddConsent(new DateTime(2025, 3, 1), Guid.NewGuid());
        participant.AddConsent(new DateTime(2025, 4, 1), Guid.NewGuid());

        var consents = participant.Consents.OrderBy(c => c.Lifetime.StartDate).ToList();

        consents[0].Lifetime.StartDate.ShouldBe(new DateTime(2025, 3, 1));
        consents[0].Lifetime.EndDate.ShouldBe(new DateTime(2025, 4, 1));

        consents[1].Lifetime.StartDate.ShouldBe(new DateTime(2025, 4, 1));
        consents[1].Lifetime.EndDate.ShouldBe(DateTime.MaxValue.Date);
    }
}
