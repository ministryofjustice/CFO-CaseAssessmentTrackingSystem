using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums
{
    public class PriStatus : SmartEnum<PriStatus>
    {
        public static readonly PriStatus Created = new(nameof(Created), 0);
        public static readonly PriStatus Accepted = new(nameof(Accepted), 1);
        public static readonly PriStatus Completed = new(nameof(Completed), 2);
        public static readonly PriStatus Abandoned = new(nameof(Abandoned), 3);

        private PriStatus(string name, int value)
            : base(name, value) { }
    }
}