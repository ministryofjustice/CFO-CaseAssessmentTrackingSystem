using System.Data;
using Ardalis.SmartEnum;
using Dapper;

namespace Cfo.Cats.Infrastructure.Persistence.Converters;

public class SmartEnumIntHandler<TEnum> : SqlMapper.TypeHandler<TEnum> where TEnum : SmartEnum<TEnum, int>
{
    public override void SetValue(IDbDataParameter parameter, TEnum? value) => parameter?.Value = value?.Value;

    public override TEnum Parse(object value) => SmartEnum<TEnum, int>.FromValue(Convert.ToInt32(value));
}