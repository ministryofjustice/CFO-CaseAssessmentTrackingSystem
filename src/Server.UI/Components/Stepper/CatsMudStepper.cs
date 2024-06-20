using MudExtensions;

namespace Cfo.Cats.Server.UI.Components.Stepper;

public class CatsMudStepper : MudStepperExtended
{
    public MudStepExtended ActiveStep => Steps.ElementAt(GetActiveIndex());

    public bool IsResultStep => HasResultStep() && GetActiveIndex().Equals(Steps.Count);

    /// <summary>
    /// Callback for step changes. A return value of true indicates success and will
    /// continue with stepper navigation.
    /// </summary>
    [Parameter]
    public Func<StepChangeDirection, int, Task<bool>> OnChangeAsync { get; set; } = 
        (StepChangeDirection direction, int index) => Task.FromResult(true);

    protected override void OnInitialized()
    {
        base.PreventStepChangeAsync += async (StepChangeDirection direction, int targetIndex) =>
        {
            var cancelled = await PreventStepChangeAsync(direction, targetIndex);

            if(cancelled is false)
            {
                cancelled = await OnChangeAsync(direction, targetIndex) is false;
            }

            return cancelled;
        };

        base.OnInitialized();
    }

    private new async Task<bool> PreventStepChangeAsync(StepChangeDirection direction, int targetIndex)
    {
        if (IsResultStep)
        {
            return await Task.FromResult(true);
        }

        // Always allow the user to step backwards.
        if (direction is StepChangeDirection.Backward)
        {
            return await Task.FromResult(false);
        }

        // Validate component if required.
        if (ActiveStep is CatsMudStep step)
        {
            var valid = await step.Condition!.Invoke();
            return await Task.FromResult(valid is false);
        }

        return await Task.FromResult(false);
    }

}