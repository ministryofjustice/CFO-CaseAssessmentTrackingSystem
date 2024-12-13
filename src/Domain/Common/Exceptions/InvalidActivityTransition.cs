using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Common.Exceptions;

public class InvalidActivityTransition(ActivityStatus from, ActivityStatus to)
    : Exception($"Activities cannot transition from {from.Name} to {to.Name}");