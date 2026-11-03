#nullable enable
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Activities;

public class EmploymentActivityTests
{
    private static Activity.ActivityContext CreateContext()
    {
        var contract = Contract.Create("C001", 1, "Test Contract", "T001", DateTime.Today, DateTime.Today.AddYears(1));
        var location = Location.Create("Test Wing", GenderProvision.Male.Value, LocationType.Wing, "C001", DateTime.Today, DateTime.Today.AddYears(1));
        var task = ObjectiveTask.Create("Task", DateTime.Today.AddDays(7));

        return new Activity.ActivityContext(
            ActivityDefinition.EmploymentinCommunityHub,
            "P12345678",
            task,
            location,
            contract,
            location,
            contract,
            EnrolmentStatus.ApprovedStatus,
            DateTime.Today,
            "1.",
            "owner-id");
    }

    [Test]
    public void Create_ShouldStoreSector()
    {
        var activity = EmploymentActivity.Create(
            CreateContext(),
            "Full time",
            "Acme Ltd",
            "Education",
            "Teachers",
            "2314",
            20000,
            "Per year",
            DateTime.Today);

        activity.Sector.ShouldBe("Education");
    }

    [Test]
    public void Create_WhenSectorIsNull_ShouldStoreNullSector()
    {
        var activity = EmploymentActivity.Create(
            CreateContext(),
            "Full time",
            "Acme Ltd",
            null,
            "Teachers",
            "2314",
            null,
            null,
            DateTime.Today);

        activity.Sector.ShouldBeNull();
    }
}
