using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Locations.Queries.GetAll;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Tenants;

public partial class TenantLocations
{
    private LocationDto[] _locations = [];
    
    private bool _loading;

    [Parameter, EditorRequired] public string TenantId { get; set; } = null!;

    [CascadingParameter] private UserProfile? UserProfile { get; set; }
    private string _currentTenantId = string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (TenantId != _currentTenantId)
            {
                _loading = true;
                _currentTenantId = TenantId;
                await LoadData();
            }
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadData()
    {
        var mediator = GetNewMediator();

        var request = new GetAllLocationsQuery()
        {
            UserProfile = UserProfile,
            TenantId = TenantId
        };

        var result = await mediator.Send(request);
        if (result.Succeeded)
        {
            _locations = result.Data!;
        }
        else
        {
            _locations = [];
        }
    }

    private string GetIcon(GenderProvision genderProvision)
    {
        if (genderProvision == GenderProvision.Male)
        {
            return Icons.Material.Filled.Male;
        }

        if (genderProvision == GenderProvision.Female)
        {
            return Icons.Material.Filled.Female;
        }

        return Icons.Material.Filled.Person;
    }

    private string GetLocationIcon(LocationDto location)
    {
        if (location.LocationType == LocationType.Female)
        {
            return Icons.Material.Filled.LocationCity;
        }

        if (location.LocationType == LocationType.Wing)
        {
            return Icons.Material.Filled.Warehouse;
        }

        if (location.LocationType == LocationType.Outlying)
        {
            return Icons.Material.Filled.Domain;
        }

        if (location.LocationType == LocationType.Feeder)
        {
            return Icons.Material.Filled.Business;
        }

        if (location.LocationType == LocationType.Community)
        {
            return Icons.Material.Filled.People;
        }

        if (location.LocationType == LocationType.Hub)
        {
            return Icons.Material.Filled.HomeWork;
        }

        if (location.LocationType == LocationType.Satellite)
        {
            return Icons.Material.Filled.SatelliteAlt;
        }

        return Icons.Material.Filled.Castle;
    }
}
