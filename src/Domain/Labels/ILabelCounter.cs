namespace Cfo.Cats.Domain.Labels;

public interface ILabelCounter
{
    /// <summary>
    /// Returns a count of the number of the Labels visible with this name for the contract.
    ///
    /// If contractId is null then it will be labels at the global level. 
    /// </summary>
    /// <param name="name">The name of the label</param>
    /// <param name="contractId">The id of the contract, or null for global labels</param>
    /// <returns>A count of visible labels with the given name and contract id.</returns>
    int CountVisibleLabels(string name, string? contractId);

    /// <summary>
    /// Counts the number of participants associated with the given label.
    /// </summary>
    /// <param name="labelId">The id of the label</param>
    /// <returns>A count of participants linked.</returns>
    int CountParticipants(LabelId labelId);
}