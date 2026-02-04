using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class LabelCannotBeRenamedIfExistingLabelExists(string newName, string oldName, string? contractId,  ILabelCounter labelCounter) : IBusinessRule
{
    public string Message => $"Cannot rename label from {oldName} to {newName} as the label already exists.";

    public bool IsBroken() => labelCounter.CountVisibleLabels(newName, contractId) > 0;
}