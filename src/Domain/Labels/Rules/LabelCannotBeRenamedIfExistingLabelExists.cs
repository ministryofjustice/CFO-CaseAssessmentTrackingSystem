using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class LabelCannotBeRenamedIfExistingLabelExists(string newName, string oldName, ILabelCounter labelCounter) : IBusinessRule
{
    public string Message => $"Cannot rename label from {oldName} to {newName} as the label already exists.";

    public bool IsBroken() => labelCounter.CountLabelsWithName(newName) > 0;
}
