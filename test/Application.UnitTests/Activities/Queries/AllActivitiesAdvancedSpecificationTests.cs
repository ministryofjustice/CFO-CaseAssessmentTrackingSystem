#nullable enable
using Ardalis.Specification.EntityFrameworkCore;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Entities.Participants;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Activities.Queries;

public class AllActivitiesAdvancedSpecificationTests
{
        [Test]
        public void LegacyExportQuery_ShouldDeserializeWithoutCategoryFilter()
        {
                const string legacyQuery = """
                        {
                            "UserProfile": {
                                "UserName": "reviewer",
                                "Email": "reviewer@example.com",
                                "UserId": "reviewer-id",
                                "TenantId": "1."
                            },
                            "TypeFilter": 5
                        }
                        """;

                var query = JsonConvert.DeserializeObject<AllActivitiesWithPagination.Query>(legacyQuery);

                query.ShouldNotBeNull();
                query.TypeFilter.ShouldBe(ActivityType.SupportWork.Value);
                query.CategoryFilter.ShouldBeNull();
        }

    [Test]
    public void CategoryFilter_WhenCategoryDoesNotBelongToType_ShouldBeRejected()
    {
        var query = new AllActivitiesWithPagination.Query
        {
            UserProfile = new UserProfile
            {
                UserName = "reviewer",
                Email = "reviewer@example.com",
                UserId = "reviewer-id",
                TenantId = "1."
            },
            TypeFilter = ActivityType.SupportWork.Value,
            CategoryFilter = ActivityCategory.EducationAndTraining.Value,
            OrderBy = "Created",
            SortDirection = "Descending"
        };

        var result = new AllActivitiesWithPagination.Handler.Validator().Validate(query);

        result.IsValid.ShouldBeFalse();
        result.Errors.Select(error => error.PropertyName).ShouldContain(nameof(query.CategoryFilter));
    }

    [Test]
    public void SelectedTypeAndCategory_ShouldRestrictTenantHierarchyResults()
    {
        var matchingRoot = CreateActivity("1.2.", ActivityDefinition.AccessingHealthSupportCustody, "root-match");
        var matchingDescendant = CreateActivity("1.2.3.", ActivityDefinition.AccessingHealthSupportCustody, "descendant-match");
        var wrongCategory = CreateActivity("1.2.4.", ActivityDefinition.AddictionCustody, "wrong-category");
        var siblingTenant = CreateActivity("1.3.", ActivityDefinition.AccessingHealthSupportCustody, "sibling-tenant");
        var outsideUserScope = CreateActivity("2.", ActivityDefinition.AccessingHealthSupportCustody, "outside-user-scope");

        var results = ApplyFilter(
            [matchingRoot, matchingDescendant, wrongCategory, siblingTenant, outsideUserScope],
            selectedTenantId: "1.2.",
            type: ActivityType.SupportWork,
            category: ActivityCategory.AccessingHealthSupport);

        results.Select(activity => activity.ParticipantId).ToHashSet()
            .ShouldBe(new HashSet<string> { "root-match", "descendant-match" });
    }

    [Test]
    public void SelectedTypeWithoutCategory_ShouldIncludeEveryCategoryOfThatType()
    {
        var firstCategory = CreateActivity("1.2.", ActivityDefinition.AccessingHealthSupportCustody, "first-category");
        var secondCategory = CreateActivity("1.2.1.", ActivityDefinition.AddictionCustody, "second-category");
        var differentType = CreateActivity("1.2.2.", ActivityDefinition.EducationandTrainingHub, "different-type");

        var results = ApplyFilter(
            [firstCategory, secondCategory, differentType],
            selectedTenantId: "1.2.",
            type: ActivityType.SupportWork,
            category: null);

        results.Select(activity => activity.ParticipantId).ToHashSet()
            .ShouldBe(new HashSet<string> { "first-category", "second-category" });
    }

    private static Activity[] ApplyFilter(
        Activity[] activities,
        string selectedTenantId,
        ActivityType type,
        ActivityCategory? category)
    {
        var filter = new AllActivitiesAdvancedFilter
        {
            UserProfile = new UserProfile
            {
                UserName = "reviewer",
                Email = "reviewer@example.com",
                UserId = "reviewer-id",
                TenantId = "1."
            },
            TenantId = selectedTenantId,
            TypeFilter = type.Value,
            CategoryFilter = category?.Value
        };

        return SpecificationEvaluator.Default
            .GetQuery(activities.AsQueryable(), new AllActivitiesAdvancedSpecification(filter))
            .ToArray();
    }

    private static Activity CreateActivity(string tenantId, ActivityDefinition definition, string participantId)
    {
        var participant = Participant.CreateFrom(
            participantId,
            "First",
            null,
            "Last",
            null,
            DateTime.UtcNow,
            null,
            "Test",
            null,
            1,
            null,
            null,
            null);

        return new TestActivity(tenantId, definition, participant, participantId);
    }

    private sealed class TestActivity : Activity
    {
        public TestActivity(string tenantId, ActivityDefinition definition, Participant participant, string participantId)
        {
            Definition = definition;
            Type = definition.Type;
            Category = definition.Category;
            TenantId = tenantId;
            Participant = participant;
            ParticipantId = participantId;
            Status = ActivityStatus.PendingStatus;
        }
    }
}