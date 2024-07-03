namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public enum RagState
{
    Unknown = 0,
    Red = 1,
    Amber = 2,
    Green = 3
}

public static class RagStateHelper
{
    public static RagState GetRagStateForScore(double score) => score switch
    {
        < 10 => RagState.Red,
        >= 10 and <= 26 => RagState.Amber,
        _ => RagState.Green
    };
}