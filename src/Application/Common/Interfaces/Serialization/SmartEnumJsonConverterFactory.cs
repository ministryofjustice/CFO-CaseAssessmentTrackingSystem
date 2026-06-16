using Ardalis.SmartEnum;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Common.Interfaces.Serialization;

/// <summary>
/// A <see cref="JsonConverterFactory"/> that serializes <see cref="SmartEnum{TEnum}"/> types
/// as their integer value, mirroring the Dapper/EF type handler behaviour.
/// </summary>
public class SmartEnumJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        for (var t = typeToConvert; t is not null; t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(SmartEnum<,>))
            {
                return true;
            }
        }
        return false;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(SmartEnumJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class SmartEnumJsonConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : SmartEnum<TEnum, int>
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => SmartEnum<TEnum, int>.FromValue(reader.GetInt32());

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.Value);
    }
}
