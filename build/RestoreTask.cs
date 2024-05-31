using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Frosting;

namespace Build;

[TaskName("Restore")]
[IsDependentOn(typeof(CleanTask))]
public class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore("../cats.sln", new DotNetRestoreSettings()
        {
            
        });
    }
}