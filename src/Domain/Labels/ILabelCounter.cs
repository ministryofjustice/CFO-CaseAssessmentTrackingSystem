namespace Cfo.Cats.Domain.Labels;

public interface ILabelCounter
{
    /// <summary>
    /// Returns a count of the number of labels that already use the given name.
    ///
    /// Label names must be globally unique, regardless of the contracts they are
    /// associated with.
    /// </summary>
    /// <param name="name">The name of the label</param>
    /// <returns>A count of labels with the given name.</returns>
    int CountLabelsWithName(string name);

    /// <summary>
    /// Counts the number of participants associated with the given label.
    /// </summary>
    /// <param name="labelId">The id of the label</param>
    /// <returns>A count of participants linked.</returns>
    int CountParticipants(LabelId labelId);
}
