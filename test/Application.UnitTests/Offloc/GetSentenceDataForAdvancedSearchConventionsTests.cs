using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;
using Cfo.Cats.Application.SecurityConstants;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Offloc;

public class GetSentenceDataForAdvancedSearchConventionsTests
{
    [Test]
    public void Query_HasParticipantAdvancedSearchPolicy()
    {
        var attribute = (RequestAuthorizeAttribute)Attribute.GetCustomAttribute(
            typeof(GetSentenceDataForAdvancedSearchQuery), typeof(RequestAuthorizeAttribute));

        attribute.ShouldNotBeNull();
        attribute!.Policy.ShouldBe(SecurityPolicies.ParticipantAdvancedSearch);
    }
}
