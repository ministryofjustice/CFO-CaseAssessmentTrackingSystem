using System.Reflection;
using WebApp.Attributes;

namespace Server.UI.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value) => value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<MudAttribute>()!
                        .DisplayName;

    public static string GetIcon(this Enum value) => value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<MudAttribute>()!
                        .Icon;

    public static Color GetColor(this Enum value) => value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<MudAttribute>()!
                        .Color;
}
