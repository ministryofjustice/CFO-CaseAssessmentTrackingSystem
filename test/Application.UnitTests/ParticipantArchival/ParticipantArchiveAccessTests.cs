#nullable enable
using Cfo.Cats.Application.Features.Participants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Participants;

public class ParticipantArchiveAccessTests
{
    private const string OwnerId = "user-owner-1";
    private const string OwnerTenantId = "1.1.1.";

    [Test]
    public void CanAccess_WhenCurrentUserIsOwner_ReturnsTrue() =>
        ParticipantArchiveAccess.CanAccess(
            currentUserId: OwnerId,
            currentUserTenantId: OwnerTenantId,
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanAccess_WhenUserIsAtHigherTenantLevel_ReturnsTrue() =>
        ParticipantArchiveAccess.CanAccess(
            currentUserId: "user-manager",
            currentUserTenantId: "1.1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanAccess_WhenUserIsAtSameTenantLevel_ReturnsTrue() =>
        ParticipantArchiveAccess.CanAccess(
            currentUserId: "user-peer",
            currentUserTenantId: OwnerTenantId,
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanAccess_WhenUserIsAtRootTenantLevel_ReturnsTrue() =>
        ParticipantArchiveAccess.CanAccess(
            currentUserId: "systemuser",
            currentUserTenantId: "1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanAccess_WhenUserIsAtLowerTenantLevel_ReturnsFalse() =>
        ParticipantArchiveAccess.CanAccess(
            currentUserId: "user-junior",
            currentUserTenantId: OwnerTenantId + "1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeFalse();

    [Test]
    public void CanAccess_WhenUserIsInDifferentTenantBranch_ReturnsFalse() =>
        ParticipantArchiveAccess.CanAccess(
            currentUserId: "user-other-branch",
            currentUserTenantId: "1.1.3.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeFalse();

}
