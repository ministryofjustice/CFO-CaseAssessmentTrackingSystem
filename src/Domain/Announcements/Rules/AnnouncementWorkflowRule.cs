using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Announcements.Rules;

public sealed class AnnouncementWorkflowRule(bool isBroken, string message) : IBusinessRule
{
    public bool IsBroken() => isBroken;
    public string Message => message;
}
