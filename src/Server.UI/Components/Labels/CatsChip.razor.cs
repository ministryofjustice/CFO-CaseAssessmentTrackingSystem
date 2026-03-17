using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Components.Labels;

public partial class CatsChip
{
    [Parameter, EditorRequired] public string Label { get; set; }

    [Parameter, EditorRequired] public AppVariant Variant { get; set; }

    [Parameter, EditorRequired] public AppColour Colour { get; set; }

    [Parameter, EditorRequired] public AppIcon Icon { get; set; }

    [Parameter, EditorRequired] public string Description { get; set; }

    [Parameter] public bool ShowDelete { get; set; } = false;

    [Parameter]
    public EventCallback<MudChip<string>> OnClose { get; set;} 

    [Parameter]
    public bool Disabled {get;set;} = false;
}