using System.ComponentModel;
using System.Reflection;

namespace Cfo.Cats.Infrastructure.PermissionSet;

public static partial class Permissions
{
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (
            var prop in typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(c =>
                    c.GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                    )
                )
        )
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
            {
                permissions.Add((string)propertyValue);
            }
        }

        return permissions;
    }

}
