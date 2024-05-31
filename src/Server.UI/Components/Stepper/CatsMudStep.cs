using MudExtensions;

namespace Cfo.Cats.Server.UI.Components.Stepper;

public class CatsMudStep : MudStep
{
    [Parameter]
    public Func<bool>? Condition { get; set; }
}