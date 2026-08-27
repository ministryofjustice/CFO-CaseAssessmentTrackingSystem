using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Entities.Participants.Rules;

public class NewAssigneeMustBeDifferentRule(string? currentUserId, string newUserId) : IBusinessRule
{
    public string Message => "The new assignee must be a different user";

    public bool IsBroken() => currentUserId == newUserId;
}
