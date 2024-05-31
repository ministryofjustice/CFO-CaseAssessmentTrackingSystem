using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace Build;

[TaskName("Publish")]
[IsDependentOn(typeof(TestTask))]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetPublish("../src/Server.UI/Server.UI.csproj", new DotNetPublishSettings()
        {
            NoBuild = true, // we are dependent on the build task
            NoRestore = true,
            Configuration = context.MsBuildConfiguration,
            OutputDirectory = "../publish/Server.Ui"
        });
    }
}