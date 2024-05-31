using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Common.Contracts;

internal interface ILifetimeEntity
{
    Lifetime Lifetime { get; }
}
