using System.Reflection;
using AutoMapper;
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Announcements.Commands;
using Cfo.Cats.Application.Features.Announcements.Queries;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementAccessTests
{
    [Test]
    public void InternalReaderQueryRejectsExternalUsersBeforeAccessingData()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns("provider");
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var users = new Mock<IUserService>();
        users.SetupGet(x => x.DataSource).Returns(
            [new ApplicationUserDto { Id = "provider", UserName = "provider@example.com" }]);
        var handler = new GetAnnouncements.Handler(unitOfWork.Object, currentUser.Object,
            users.Object, new Mock<IMapper>().Object);

        Should.Throw<ForbiddenException>(async () => await handler.Handle(
            new GetAnnouncements.Query { Audience = AnnouncementAudience.InternalStaff }, CancellationToken.None));

        unitOfWork.VerifyGet(x => x.DbContext, Times.Never);
    }

    [TestCase(typeof(SaveAnnouncement.Command))]
    [TestCase(typeof(ApproveAnnouncement.Command))]
    [TestCase(typeof(DeleteAnnouncement.Command))]
    [TestCase(typeof(GetAnnouncementsForManagement.Query))]
    public void ManagementRequestsUseExistingSeniorPolicy(Type type)
        => type.GetCustomAttribute<RequestAuthorizeAttribute>()!.Policy.ShouldBe(SecurityPolicies.SeniorInternal);

    [Test]
    public void ReaderRequestsAllowAllAuthorizedUsers()
        => typeof(GetAnnouncements.Query).GetCustomAttribute<RequestAuthorizeAttribute>()!
            .Policy.ShouldBe(SecurityPolicies.AuthorizedUser);

    [TestCase("staff@justice.gov.uk", true)]
    [TestCase("STAFF@JUSTICE.GOV.UK", true)]
    [TestCase("staff@provider.org", false)]
    [TestCase("staff@justice.gov.uk.provider.org", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void InternalClassificationUsesTheExistingClaimRule(string userName, bool expected)
        => UserProfile.IsInternalUser(userName).ShouldBe(expected);

    [Test]
    public void InternalClassificationDoesNotRequireAnInternalRoleOrAnUnlockedAccount()
    {
        var user = new ApplicationUserDto
        {
            Id = "internal",
            UserName = "staff@justice.gov.uk",
            AssignedRoles = [],
            LockoutEnd = DateTimeOffset.MaxValue
        };
        Should.NotThrow(() => Cfo.Cats.Application.Features.Announcements.AnnouncementAccess.EnsureAudience(
            user, AnnouncementAudience.InternalStaff));
    }

    [Test]
    public void AnInternalRoleAloneDoesNotExposeInternalAnnouncements()
    {
        var user = new ApplicationUserDto
        {
            Id = "provider",
            UserName = "staff@provider.org",
            AssignedRoles = [RoleNames.SystemSupport, RoleNames.SMT]
        };
        Should.Throw<ForbiddenException>(() => Cfo.Cats.Application.Features.Announcements.AnnouncementAccess.EnsureAudience(
            user, AnnouncementAudience.InternalStaff));
    }
}
