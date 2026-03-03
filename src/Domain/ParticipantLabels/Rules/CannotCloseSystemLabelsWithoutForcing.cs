using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Domain.ParticipantLabels.Rules;

public class CannotCloseSystemLabelsWithoutForcing(Label label, bool isForced) : IBusinessRule
{
    public bool IsBroken() => label.Scope == LabelScope.System && isForced == false;

    public string Message => "System labels cannot be manually closed.";
}