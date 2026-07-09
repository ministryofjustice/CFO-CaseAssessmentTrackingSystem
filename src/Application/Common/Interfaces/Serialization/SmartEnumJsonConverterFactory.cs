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
        => TryGetIntSmartEnumType(typeToConvert, out _);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (!TryGetIntSmartEnumType(typeToConvert, out var smartEnumType))
        {
            return null;
        }

        var converterType = typeof(SmartEnumJsonConverter<>).MakeGenericType(smartEnumType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    /// <summary>
    /// Resolves the declared <c>SmartEnum&lt;TEnum, int&gt;</c> type for a candidate type.
    /// SmartEnum members are declared as nested subclasses (e.g. <c>EnrolmentStatus.Approved</c>),
    /// so the runtime type of a value is a derived type rather than the enum type itself. The
    /// converter must be created against the enum type named in the <c>SmartEnum&lt;,&gt;</c> base
    /// (the first generic argument) to satisfy the <c>TEnum : SmartEnum&lt;TEnum, int&gt;</c> constraint.
    /// Only integer-valued SmartEnums are supported.
    /// </summary>
    private static bool TryGetIntSmartEnumType(Type typeToConvert, out Type smartEnumType)
    {
        for (var t = typeToConvert; t is not null; t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(SmartEnum<,>))
            {
                var genericArguments = t.GetGenericArguments();
                if (genericArguments[1] == typeof(int))
                {
                    smartEnumType = genericArguments[0];
                    return true;
                }

                break;
            }
        }

        smartEnumType = null!;
        return false;
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
