using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Domain.Common.Contracts;

internal interface ILifetime
{
    Lifetime Lifetime { get; }
}
