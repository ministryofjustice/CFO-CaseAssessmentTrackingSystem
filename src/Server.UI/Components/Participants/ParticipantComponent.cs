namespace Cfo.Cats.Server.UI.Components.Participants;

public abstract class ParticipantComponent<TData>
    : CatsComponent<TData> where TData : class
{
    /// <summary>
    /// The identifier of the participant
    /// </summary>
    [Parameter, EditorRequired]
    public string ParticipantId { get; set; } = null!;
}
