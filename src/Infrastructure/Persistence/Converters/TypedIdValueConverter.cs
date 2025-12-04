using Cfo.Cats.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cfo.Cats.Infrastructure.Persistence.Converters;

public class TypedIdValueConverter<T> : ValueConverter<T, Guid>
    where T : TypedIdValueBase
{
    public TypedIdValueConverter(Func<Guid, T> factory)
        : base(id => id.Value, value => factory(value))
    {}
}