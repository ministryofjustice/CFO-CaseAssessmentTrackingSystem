using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

public partial class ActivityTypeCategoryFilterDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public int? TypeFilter { get; set; }

    [Parameter]
    public int? CategoryFilter { get; set; }

    private int? SelectedType { get; set; }
    private int? SelectedCategory { get; set; }

    private bool CategoryDisabled => SelectedType is null
        || SelectedType == ActivityType.EducationAndTraining.Value
        || SelectedType == ActivityType.Employment.Value;

    private IReadOnlyList<ActivityCategory> Categories => SelectedType is null
        ? []
        : ActivityDefinition.List
            .Where(definition => definition.Type.Value == SelectedType)
            .Select(definition => definition.Category)
            .Distinct()
            .OrderBy(category => category.Name)
            .ToArray();

    protected override void OnInitialized()
    {
        SelectedType = TypeFilter;

        if (CategoryDisabled is false && CategoryFilter.HasValue)
        {
            SelectedCategory = Categories.Any(category => category.Value == CategoryFilter.Value)
                ? CategoryFilter
                : null;
        }
    }

    private Task OnTypeChanged(int? type)
    {
        SelectedType = type;
        SelectedCategory = null;
        return Task.CompletedTask;
    }

    private Task OnCategoryChanged(int? category)
    {
        SelectedCategory = category;
        return Task.CompletedTask;
    }

    private void Cancel() => MudDialog.Cancel();

    private void Submit() => MudDialog.Close(DialogResult.Ok(new ActivityTypeCategorySelection(
        SelectedType,
        SelectedCategory)));
}

public sealed record ActivityTypeCategorySelection(int? TypeFilter, int? CategoryFilter);