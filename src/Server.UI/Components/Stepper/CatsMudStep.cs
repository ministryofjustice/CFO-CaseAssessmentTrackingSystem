using MudExtensions;

namespace Cfo.Cats.Server.UI.Components.Stepper;

public class CatsMudStep : MudStep
{
    [Parameter]
    public Func<Task<bool>>? Condition { get; set; }
}