using System.Reflection;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Announcements.Commands;
using Cfo.Cats.Application.Features.Announcements.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Domain.Common.Exceptions;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Announcements;

public class AnnouncementTypeTests
{
    [Test]
    public void TypeNamesAreTrimmedAndTypesHaveAnIdentity()
    {
        var type = AnnouncementType.Create(" Team Update ");
        type.Name.ShouldBe("Team Update");
        type.Id.ShouldNotBe(Guid.Empty);
    }

    [TestCase("")]
    [TestCase(" ")]
    public void EmptyTypeNamesAreRejected(string name)
        => Should.Throw<BusinessRuleValidationException>(() => AnnouncementType.Create(name));

    [Test]
    public void RenamingTrimsTheNameAndKeepsTheIdentity()
    {
        var type = AnnouncementType.Create("General");
        var id = type.Id;
        type.Rename(" Team Update ");
        type.Name.ShouldBe("Team Update");
        type.Id.ShouldBe(id);
        Should.Throw<BusinessRuleValidationException>(() => type.Rename(" "));
        Should.Throw<BusinessRuleValidationException>(() =>
            type.Rename(new string('a', AnnouncementConstants.TypeNameMaximumLength + 1)));
        type.Name.ShouldBe("Team Update");
    }

    [Test]
    public void ManagementCommandsValidateIdentityAndNames()
    {
        new RenameAnnouncementType.Validator().Validate(new RenameAnnouncementType.Command()).IsValid.ShouldBeFalse();
        new DeleteAnnouncementType.Validator().Validate(new DeleteAnnouncementType.Command()).IsValid.ShouldBeFalse();
        new RenameAnnouncementType.Validator().Validate(new RenameAnnouncementType.Command
        {
            Id = Guid.NewGuid(), OriginalName = "General", Name = " "
        }).IsValid.ShouldBeFalse();
        new RenameAnnouncementType.Validator().Validate(new RenameAnnouncementType.Command
        {
            Id = Guid.NewGuid(), OriginalName = "General", Name = "Update"
        }).IsValid.ShouldBeTrue();
    }

    [Test]
    public void TypeNameLengthLimitIsEnforced()
    {
        AnnouncementType.Create(new string('a', AnnouncementConstants.TypeNameMaximumLength)).Name.Length
            .ShouldBe(AnnouncementConstants.TypeNameMaximumLength);
        Should.Throw<BusinessRuleValidationException>(() =>
            AnnouncementType.Create(new string('a', AnnouncementConstants.TypeNameMaximumLength + 1)));
        new AddAnnouncementType.Validator().Validate(new AddAnnouncementType.Command
            { Name = new string('a', AnnouncementConstants.TypeNameMaximumLength + 1) }).IsValid.ShouldBeFalse();
    }

    [TestCase(typeof(AddAnnouncementType.Command))]
    [TestCase(typeof(RenameAnnouncementType.Command))]
    [TestCase(typeof(DeleteAnnouncementType.Command))]
    [TestCase(typeof(GetAnnouncementTypes.Query))]
    public void TypeRequestsRequireSeniorUsers(Type request)
        => request.GetCustomAttribute<RequestAuthorizeAttribute>()!.Policy.ShouldBe(SecurityPolicies.SeniorInternal);
}
