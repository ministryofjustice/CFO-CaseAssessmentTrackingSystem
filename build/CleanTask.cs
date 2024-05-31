using Cake.Common.IO;
using Cake.Frosting;

namespace Build;

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        // clean any existing publishing output
        context.CleanDirectories("../publish");
        
        context.CleanDirectory($"../src/Server.Ui/bin/{context.MsBuildConfiguration}");
        context.CleanDirectory($"../src/Server/bin/{context.MsBuildConfiguration}");
        context.CleanDirectory($"../src/Migrators/Migrators.MSSQL/bin/{context.MsBuildConfiguration}");
        context.CleanDirectory($"../src/Migrators/Migrators.SqLite/bin/{context.MsBuildConfiguration}");
        context.CleanDirectory($"../src/Infrastructure/bin/{context.MsBuildConfiguration}");
        context.CleanDirectory($"../src/Application/bin/{context.MsBuildConfiguration}");
        context.CleanDirectory($"../src/Domain/bin/{context.MsBuildConfiguration}");
    }
}