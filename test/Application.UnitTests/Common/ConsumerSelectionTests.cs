using Cfo.Cats.Infrastructure.Services.MessageHandling;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common;

[TestFixture]
public class ConsumerSelectionTests
{
    [Test]
    public void Empty_Options_Enables_All()
    {
        var result = ConsumerSelection.Resolve(new ConsumerSelectionOptions());

        result.ShouldBe(ConsumerNames.All, ignoreOrder: true);
    }

    [Test]
    public void Include_Restricts_To_Named_Services()
    {
        var result = ConsumerSelection.Resolve(new ConsumerSelectionOptions { Include = ["payment"] });

        result.ShouldBe([ConsumerNames.Payment]);
    }

    [Test]
    public void Exclude_Removes_From_All()
    {
        var result = ConsumerSelection.Resolve(new ConsumerSelectionOptions { Exclude = [ConsumerNames.Payment] });

        result.ShouldNotContain(ConsumerNames.Payment);
        result.Count.ShouldBe(ConsumerNames.All.Count - 1);
    }

    [Test]
    public void Exclude_Is_Applied_After_Include()
    {
        var result = ConsumerSelection.Resolve(new ConsumerSelectionOptions
        {
            Include = [ConsumerNames.Payment, ConsumerNames.Tasks],
            Exclude = [ConsumerNames.Payment]
        });

        result.ShouldBe([ConsumerNames.Tasks]);
    }

    [TestCase("Paymnet", true)]
    [TestCase("Paymnet", false)]
    public void Unknown_Name_Throws(string name, bool include)
    {
        var options = include
            ? new ConsumerSelectionOptions { Include = [name] }
            : new ConsumerSelectionOptions { Exclude = [name] };

        Should.Throw<InvalidOperationException>(() => ConsumerSelection.Resolve(options));
    }
}
