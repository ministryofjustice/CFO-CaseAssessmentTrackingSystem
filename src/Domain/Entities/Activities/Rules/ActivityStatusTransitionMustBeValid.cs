using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.Activities.Rules;

public class ActivityStatusTransitionMustBeValid(ActivityStatus from, ActivityStatus to) : IBusinessRule
{
    public string Message => $"Activities cannot transition from {from.Name} to {to.Name}";

    public bool IsBroken() => from.CanTransitionTo(to) is false;
}
