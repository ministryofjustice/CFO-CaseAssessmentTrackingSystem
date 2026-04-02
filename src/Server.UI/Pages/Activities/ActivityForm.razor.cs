using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;
using MudBlazor.Extensions;

namespace Cfo.Cats.Server.UI.Pages.Activities;

public partial class ActivityForm
{
    private IEnumerable<ActivityDefinition> _activities = [];
    private MudAutocomplete<ActivityDefinition?> _activityDropdown = new();
    private readonly bool _uploading = false;
    private IReadOnlyCollection<ActivityType> _filteredTypes = [];
    private bool _hasSelectedActivityAlreadyBeenAddedOnThisDate;
    private bool Disabled => Model.Location is null;
    private bool _hasParticipantBeenAtThisLocationOnThisDate = true;
    [Parameter, EditorRequired] public required AddActivity.Command Model { get; set; }

    [Parameter] public string ParticipantId { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        SetActivities();
        base.OnInitialized();
    }

    private void SetActivities() => _activities = Model.Location is null
        ? []
        : ActivityDefinition.GetActivitiesForLocation(Model.Location.LocationType);

    private async Task OnLocationChanged(LocationDto? location)
    {
        Model.Location = location;
        LocationChanged();
    }

    private void LocationChanged()
    {
        SetActivities();
        _filteredTypes = [];
        ClearSelectedActivity();
        ClearSelectedActivityDate();
    }

    private async Task OnActivityChanged()
    {
        _hasSelectedActivityAlreadyBeenAddedOnThisDate = Model.Definition is not null && await GetNewMediator().Send(
            new ExistsByCategory.Query()
            {
                Category = Model.Definition.Category,
                ParticipantId = Model.ParticipantId,
                CommencedOn = Model.CommencedOn
            });

        ClearTemplates();
    }

    private async Task OnDateChanged()
    {
        if (Model.CommencedOn.HasValue)
        {
            _hasSelectedActivityAlreadyBeenAddedOnThisDate = Model.Definition is not null && await GetNewMediator()
                .Send(new ExistsByCategory.Query()
                {
                    Category = Model.Definition.Category,
                    ParticipantId = Model.ParticipantId,
                    CommencedOn = Model.CommencedOn
                });

            await CheckParticipantBeenAtThisLocationOnDate();
        }

        ClearTemplates();
    }

    private async Task CheckParticipantBeenAtThisLocationOnDate() => _hasParticipantBeenAtThisLocationOnThisDate = await GetNewMediator().Send(
            new GetParticipantWasAtThisLocationCheck.Query()
            {
                ParticipantId = Model.ParticipantId,
                LocationId = Model.Location!.Id,
                DateAtLocation = Model.CommencedOn
            });

    private void ClearTemplates()
    {
        Model.ISWTemplate = new() { ParticipantId = Model.ParticipantId };
        Model.EducationTrainingTemplate = new() { ParticipantId = Model.ParticipantId };
        Model.EmploymentTemplate = new() { ParticipantId = Model.ParticipantId };
    }

    private void ClearSelectedActivity()
    {
        if (_activityDropdown.GetState(x => x.Value) is not null)
        {
            _activityDropdown.ResetAsync();
        }
    }

    private void ClearSelectedActivityDate()
    {
        if (Model.CommencedOn is not null)
        {
            Model.CommencedOn = null;
        }
    }

    private bool IsAvailableAtSelectedLocation(ActivityType type) => _activities.Any(activity => activity.Type == type);

    private Task<IEnumerable<ActivityDefinition>> SearchActivities(string searchText,
        CancellationToken cancellationToken)
    {
        IEnumerable<ActivityDefinition> result;
        IEnumerable<ActivityDefinition> filteredActivities = _activities
            .Where(activity => _filteredTypes.Count is 0 || _filteredTypes.Contains(activity.Type))
            .OrderBy(activity => activity.Name);
        if (string.IsNullOrEmpty(searchText))
        {
            result = filteredActivities;
        }
        else
        {
            result = filteredActivities.Where(x =>
                x.Category.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(result);
    }
}