using MudExtensions;

namespace Cfo.Cats.Server.UI.Components.Stepper;

public class CatsMudStep : MudStepExtended
{
    [Parameter]
    public Func<Task<bool>>? Condition { get; set; }
}