using System.ComponentModel;

namespace Cfo.Cats.Server.UI.Models.NavigationMenu;

public enum PageStatus
{
    [Description("Coming Soon")]
    ComingSoon,

    [Description("WIP")]
    Wip,

    [Description("New")]
    New,

    [Description("Completed")]
    Completed
}
