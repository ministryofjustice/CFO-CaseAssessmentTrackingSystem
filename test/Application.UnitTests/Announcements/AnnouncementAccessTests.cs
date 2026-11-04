using System.Reflection;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Announcements.Commands;
using Cfo.Cats.Application.Features.Announcements.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementAccessTests
{
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

    [TestCase("staff@provider.org", true)]
    [TestCase("staff@justice.gov.uk", false)]
    public void ProfileAndDomainUserPreserveInternalFlagRegardlessOfEmail(string email, bool isInternalUser)
    {
        var profile = new UserProfile
        {
            UserId = "user",
            UserName = email,
            Email = email,
            IsInternalUser = isInternalUser
        };

        profile.IsInternalUser.ShouldBe(isInternalUser);
        profile.AsDomainUser().IsInternalUser.ShouldBe(isInternalUser);
    }

}
