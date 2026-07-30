using Cfo.Cats.Domain.ValueObjects;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class RagComponent
{
    [Parameter, EditorRequired]
    public PathwayScore Pathway { get; set; } = default!;

    [Parameter]
    public RenderFragment? TooltipContent { get; set; }

    private (string Background, string Foreground) GetColor(double ragScore) =>
        ragScore switch
        {
            > 25 => ("#00703C", "white"), // GOV.UK green
            >= 10 => ("#FFBF00", "black"), // amber
            < 0 => ("#B1B4B6", "black"), // GOV.UK grey
            _ => ("#D4351C", "white"), // GOV.UK red
        };
}