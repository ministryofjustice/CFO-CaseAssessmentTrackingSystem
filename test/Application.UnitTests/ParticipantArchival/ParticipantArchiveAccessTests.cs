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
    public void CanArchive_WhenCurrentUserIsOwner_ReturnsTrue() =>
        ParticipantArchiveAccess.CanArchive(
            currentUserId: OwnerId,
            currentUserTenantId: OwnerTenantId,
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanArchive_WhenUserIsAtHigherTenantLevel_ReturnsTrue() =>
        ParticipantArchiveAccess.CanArchive(
            currentUserId: "user-manager",
            currentUserTenantId: "1.1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanArchive_WhenUserIsAtSameTenantLevel_ReturnsTrue() =>
        ParticipantArchiveAccess.CanArchive(
            currentUserId: "user-peer",
            currentUserTenantId: OwnerTenantId,
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanArchive_WhenUserIsAtRootTenantLevel_ReturnsTrue() =>
        ParticipantArchiveAccess.CanArchive(
            currentUserId: "systemuser",
            currentUserTenantId: "1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeTrue();

    [Test]
    public void CanArchive_WhenUserIsAtLowerTenantLevel_ReturnsFalse() =>
        ParticipantArchiveAccess.CanArchive(
            currentUserId: "user-junior",
            currentUserTenantId: OwnerTenantId + "1.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeFalse();

    [Test]
    public void CanArchive_WhenUserIsInDifferentTenantBranch_ReturnsFalse() =>
        ParticipantArchiveAccess.CanArchive(
            currentUserId: "user-other-branch",
            currentUserTenantId: "1.1.3.",
            participantOwnerId: OwnerId,
            participantOwnerTenantId: OwnerTenantId)
        .ShouldBeFalse();

}
