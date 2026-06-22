using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Cfo.Cats.Application.Common.Interfaces.Serialization;

/// <summary>
/// Serializer options used exclusively by the Fusion distributed (Redis) cache.
/// These mirror <see cref="DefaultJsonSerializerOptions"/> but add
/// <see cref="SmartEnumJsonConverterFactory"/> so SmartEnum-valued DTOs (e.g. LocationDto)
/// round-trip through the distributed cache. They are deliberately kept separate from
/// <see cref="DefaultJsonSerializerOptions"/>, which is also used for EF Core value
/// conversions, audit trails and export payloads where the persisted/on-disk JSON format
/// must remain stable.
/// </summary>
public class CacheJsonSerializerOptions
{
    public static JsonSerializerOptions Options =>
        new()
        {
            Encoder = JavaScriptEncoder.Create(
                UnicodeRanges.BasicLatin,
                UnicodeRanges.CjkUnifiedIdeographs
            ),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), new SmartEnumJsonConverterFactory() },
        };
}
