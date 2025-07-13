using Cfo.Cats.Application.Common.Enums;

using static MudBlazor.Color;

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

    public static string AsMudIcon(this AppIcon appIcon) =>
        appIcon switch
        {
            AppIcon.Enrolment => Icons.Material.Filled.HowToReg,               // person + check
            AppIcon.Objective => Icons.Material.Filled.TrackChanges,           // crosshair/target symbol
            AppIcon.Task => Icons.Material.Filled.Task,                        // checklist
            AppIcon.Activity => Icons.Material.Filled.EventNote,               // calendar w/ notes
            AppIcon.Payment => Icons.Material.Filled.CreditCard,     // neutral wallet icon
            AppIcon.Location => Icons.Material.Filled.LocationOn,              // map pin
            _ => throw new ArgumentOutOfRangeException(nameof(appIcon), appIcon, null)
        };

    
}