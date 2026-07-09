using System.Text.Json;
using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Domain.Common.Enums;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Common.Serialization;

public class SmartEnumJsonConverterFactoryTests
{
    private static JsonSerializerOptions Options => new()
    {
        Converters = { new SmartEnumJsonConverterFactory() }
    };

    [Test]
    public void CanConvert_DeclaredSmartEnumType_ReturnsTrue()
        => new SmartEnumJsonConverterFactory()
            .CanConvert(typeof(EnrolmentStatus))
            .ShouldBeTrue();

    [Test]
    public void CanConvert_NestedMemberType_ReturnsTrue()
        => new SmartEnumJsonConverterFactory()
            .CanConvert(EnrolmentStatus.ApprovedStatus.GetType())
            .ShouldBeTrue();

    [Test]
    public void Serializing_NestedMemberValue_DoesNotThrowAndWritesIntegerValue()
    {
        var json = JsonSerializer.Serialize(EnrolmentStatus.ApprovedStatus, Options);

        json.ShouldBe(EnrolmentStatus.ApprovedStatus.Value.ToString());
    }

    [Test]
    public void RoundTrip_PreservesValue()
    {
        var json = JsonSerializer.Serialize(EnrolmentStatus.ApprovedStatus, Options);
        var result = JsonSerializer.Deserialize<EnrolmentStatus>(json, Options);

        result.ShouldBe(EnrolmentStatus.ApprovedStatus);
    }
}
