namespace Cfo.Cats.Infrastructure.PermissionSet;

using System.ComponentModel;

public static partial class Permissions
{
    [DisplayName("Job")]
    [Description("Job Permissions")]
    public static class Hangfire
    {
        public const string View = "Permissions.Hangfire.View";
        public const string Jobs = "Permissions.Hangfire.Jobs";
    }
}