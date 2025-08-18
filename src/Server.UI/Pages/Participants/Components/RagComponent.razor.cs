using Cfo.Cats.Domain.ValueObjects;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class RagComponent
{
    [Parameter, EditorRequired]
    public PathwayScore Pathway { get; set; } = default!;

    [Parameter]
    public RenderFragment? TooltipContent { get; set; }

    private Color GetColor(double ragScore) =>
        ragScore switch
        {
            > 25 => Color.Success,
            >= 10 => Color.Warning,
            _ => Color.Error,
        };
}