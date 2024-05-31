using Cake.Frosting;

namespace Build;

[IsDependentOn(typeof(PublishTask))]
public sealed class Default : FrostingTask
{
}