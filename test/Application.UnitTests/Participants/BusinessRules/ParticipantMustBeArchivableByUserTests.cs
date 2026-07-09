#nullable enable
using Cfo.Cats.Domain.Entities.Participants.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants.BusinessRules;

public class ParticipantMustBeArchivableByUserTests
{
    private const string OwnerId = "user-owner-1";
    private const string OwnerTenantId = "1.1.1.";

    [Test]
    public void IsBroken_WhenCurrentUserIsOwner_ShouldReturnFalse()
    {
        var rule = new ParticipantMustBeArchivableByUser(
            currentUserId: OwnerId,
            currentUserTenantId: OwnerTenantId,
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenUserIsAtHigherTenantLevel_ShouldReturnFalse()
    {
        var rule = new ParticipantMustBeArchivableByUser(
            currentUserId: "user-manager",
            currentUserTenantId: "1.1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenUserIsAtSameTenantLevel_ShouldReturnFalse()
    {
        var rule = new ParticipantMustBeArchivableByUser(
            currentUserId: "user-peer",
            currentUserTenantId: OwnerTenantId,
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId);

        rule.IsBroken().ShouldBeFalse();
    }

    [Test]
    public void IsBroken_WhenUserIsAtLowerTenantLevel_ShouldReturnTrue()
    {
        var rule = new ParticipantMustBeArchivableByUser(
            currentUserId: "user-junior",
            currentUserTenantId: OwnerTenantId + "1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId);

        rule.IsBroken().ShouldBeTrue();
        rule.Message.ShouldBe("You are not authorized to archive this participant");
    }

    [Test]
    public void IsBroken_WhenUserIsInDifferentTenantBranch_ShouldReturnTrue()
    {
        var rule = new ParticipantMustBeArchivableByUser(
            currentUserId: "user-other-branch",
            currentUserTenantId: "1.1.3.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId);

        rule.IsBroken().ShouldBeTrue();
    }

    [Test]
    public void IsBroken_WhenParticipantIsUnassigned_ShouldReturnFalse()
    {
        var rule = new ParticipantMustBeArchivableByUser(
            currentUserId: "user-any",
            currentUserTenantId: "1.1.3.",
            participantOwnerId: null,
            participantOwnerTenantId: null);

        rule.IsBroken().ShouldBeFalse();
    }
}