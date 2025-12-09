using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Labels.Rules;

public class LabelCannotBeDeletedIfParticipantsAreLinked(LabelId labelId, ILabelCounter labelCounter) : IBusinessRule
{
    public bool IsBroken()
    {
        var linkedParticipantsCount = labelCounter.CountParticipants(labelId);
        return linkedParticipantsCount > 0;
    }

    public string Message => "Label cannot be deleted because there are participants linked to it.";
}