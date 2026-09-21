using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Performance
{
    [Inject]
    private IParticipantDialogService ParticipantDialogService { get; set; } = null!;

    private MudDateRangePicker _picker = null!;

    private DateRange _dateRange = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);

    private int? _selectedLocationId;
    private string? _selectedLocationName;
    private string _selectedLocationType = string.Empty;
    private readonly string[] _locationTypes = LocationType.List.Select(l => l.Name).ToArray();

    private string PerformanceKey =>
        $"{SelectionKey}|{_dateRange.Start?.Ticks ?? 0}|{_dateRange.End?.Ticks ?? 0}|{_selectedLocationId}|{_selectedLocationType}";

    private string SelectedLocationLabel => _selectedLocationId is null ? "All Locations" : _selectedLocationName ?? "All Locations";

    private string SelectedLocationTypeLabel => string.IsNullOrEmpty(_selectedLocationType) ? "All Location Types" : _selectedLocationType;

    private async Task ShowLocationDialog()
    {
        var location = await ParticipantDialogService.PromptForLocationAsync(CurrentUser, showAllOption: _selectedLocationId.HasValue);

        if (location is not null)
        {
            _selectedLocationId = location.Id == 0 ? null : location.Id;
            _selectedLocationName = location.Id == 0 ? null : location.Name;
        }
    }

    private void OnLocationTypeChanged(string locationType) => _selectedLocationType = locationType;
}
