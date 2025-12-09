using Cfo.Cats.Application.Common.Enums;
using Cfo.Cats.Domain.Common.Enums;
using static MudBlazor.Color;
using static MudBlazor.Variant;

namespace Cfo.Cats.Server.UI.Extensions;

public static class EnumExtensions
{
    public static Color AsMudColour(this AppColour appColour) 
        => appColour switch
    {
        AppColour.Default => Default,
        AppColour.Primary => Primary,
        AppColour.Secondary => Secondary,
        AppColour.Info => Info,
        AppColour.Success => Success,
        AppColour.Warning => Warning,
        AppColour.Error => Error,
        AppColour.Dark => Dark,
        _ => throw new ArgumentOutOfRangeException(nameof(appColour), appColour, null)
    };

    public static Variant AsMudVariant(this AppVariant appVariant)
        => appVariant switch
        {
            AppVariant.Filled => Filled,
            AppVariant.Outlined => Outlined,
            AppVariant.Text => Text,
            _ => throw new ArgumentOutOfRangeException(nameof(appVariant), appVariant, null)
        };
    
    public static string AsMudIcon(this AppIcon appIcon) =>
        appIcon switch
        {
            AppIcon.Enrolment => Icons.Material.Filled.HowToReg,    
            AppIcon.Objective => Icons.Material.Filled.TrackChanges,
            AppIcon.Task => Icons.Material.Filled.Task,             
            AppIcon.Activity => Icons.Material.Filled.EventNote,    
            AppIcon.Payment => Icons.Material.Filled.CreditCard,    
            AppIcon.Location => Icons.Material.Filled.LocationOn,
            AppIcon.Reassignment => Icons.Material.Filled.AssignmentInd,
            AppIcon.HubInduction => Icons.Material.Filled.AssignmentInd,
            AppIcon.WingInduction => Icons.Material.Filled.AssignmentInd,
            AppIcon.WingInductionPhase => Icons.Material.Filled.AssignmentInd,
            _ => throw new ArgumentOutOfRangeException(nameof(appIcon), appIcon, null)
        };

    
}