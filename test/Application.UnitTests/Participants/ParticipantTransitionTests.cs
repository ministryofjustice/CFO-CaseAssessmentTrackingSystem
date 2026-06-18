#nullable enable
using System;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Entities.Participants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants;

public class ParticipantTransitionTests
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
    public void TransitionTo_WhenTransitionIsValid_ShouldUpdateEnrolmentStatus()
    {
        var participant = CreateParticipant();
        participant.EnrolmentStatus.ShouldBe(EnrolmentStatus.IdentifiedStatus);

        participant.TransitionTo(EnrolmentStatus.EnrollingStatus, null, null);

        participant.EnrolmentStatus.ShouldBe(EnrolmentStatus.EnrollingStatus);
    }

    [Test]
    public void TransitionTo_WhenTransitionIsInvalid_ShouldThrowBusinessRuleException()
    {
        var participant = CreateParticipant();

        Should.Throw<BusinessRuleValidationException>(() =>
                participant.TransitionTo(EnrolmentStatus.ApprovedStatus, null, null))
            .Message.ShouldBe("Participants cannot transition from Identified to Approved");
    }

    [Test]
    public void TransitionTo_WhenTransitionIsInvalid_ShouldNotChangeEnrolmentStatus()
    {
        var participant = CreateParticipant();

        Should.Throw<BusinessRuleValidationException>(() =>
            participant.TransitionTo(EnrolmentStatus.ApprovedStatus, null, null));

        participant.EnrolmentStatus.ShouldBe(EnrolmentStatus.IdentifiedStatus);
    }
}
