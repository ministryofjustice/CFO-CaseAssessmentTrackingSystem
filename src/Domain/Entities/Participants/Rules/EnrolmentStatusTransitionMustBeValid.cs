using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.Participants.Rules;

public class EnrolmentStatusTransitionMustBeValid(EnrolmentStatus from, EnrolmentStatus to) : IBusinessRule
{
    public string Message => $"Participants cannot transition from {from.Name} to {to.Name}";

    public bool IsBroken() => from.CanTransitionTo(to) is false;
}
