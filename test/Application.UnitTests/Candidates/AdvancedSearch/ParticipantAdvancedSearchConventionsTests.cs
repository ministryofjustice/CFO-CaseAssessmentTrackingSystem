using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;
using Cfo.Cats.Application.SecurityConstants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Candidates.AdvancedSearch;

public class ParticipantAdvancedSearchConventionsTests
{
    [Test]
    public void Query_HasParticipantAdvancedSearchPolicy()
    {
        var attribute = (RequestAuthorizeAttribute)Attribute.GetCustomAttribute(
            typeof(ParticipantAdvancedSearchQuery), typeof(RequestAuthorizeAttribute));

        attribute.ShouldNotBeNull();
        attribute!.Policy.ShouldBe(SecurityPolicies.ParticipantAdvancedSearch);
    }
}

