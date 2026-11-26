using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Offloc.Queries.GetSentenceDataForAdvancedSearch;
using Cfo.Cats.Server.UI.Pages.Participants.Components.DMS.Offloc;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Offloc;

public class OffLocDetailsComponentTests
{
    [Test]
    public void CreateQuery_UsesAuditedSentenceLookupQuery()
    {
#pragma warning disable BL0005
        var component = new OffLocDetails
        {
            NomisNumber = "AB1234C",
            RecordIdentifier = "AB1234567",
            Justification = "CallRef123",
            CurrentUser = new UserProfile
            {
                UserName = "test",
                Email = "test@example.com",
                UserId = "test-user"
            }
        };
#pragma warning restore BL0005

        var createQuery = typeof(OffLocDetails).GetMethod("CreateQuery", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        createQuery.ShouldNotBeNull();
        var query = createQuery!.Invoke(component, null);

        var sentenceQuery = query.ShouldBeOfType<GetSentenceDataForAdvancedSearchQuery>();
        sentenceQuery.NomisNumber.ShouldBe("AB1234C");
        sentenceQuery.RecordIdentifier.ShouldBe("AB1234567");
        sentenceQuery.Justification.ShouldBe("CallRef123");
        sentenceQuery.CurrentUser.UserId.ShouldBe("test-user");
    }
}
