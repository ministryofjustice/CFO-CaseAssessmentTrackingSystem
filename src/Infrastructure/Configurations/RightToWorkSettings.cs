using Cfo.Cats.Application.Common.Interfaces;
namespace Cfo.Cats.Infrastructure.Configurations;

public class RightToWorkSettings : IRightToWorkSettings
{
    public const string Key = nameof(RightToWorkSettings);
    public IList<string> NationalitiesExempted { get; set; } = new List<string>();

}