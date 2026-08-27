using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Entities.Participants.Rules;

public class QueueEntryMustBeOpenToReassignRule(bool isCompleted) : IBusinessRule
{
    public string Message => "Cannot reasign a completed queue entry";
    
    public bool IsBroken() => isCompleted;
}