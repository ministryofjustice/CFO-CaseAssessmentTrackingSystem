using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class GlobalRulesCanOnlyBeDeletedByInternalUsersRule(string? contractId, DomainUser domainUser) : IBusinessRule
{
    public bool IsBroken()
        => contractId switch
        {
            not null => false,
            null => !domainUser.IsInternalUser
        };

    public string Message =>  "You do not have permission to perform this action";
}