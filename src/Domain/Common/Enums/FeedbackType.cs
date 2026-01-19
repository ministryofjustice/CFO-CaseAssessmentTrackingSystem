using System.ComponentModel;

namespace Cfo.Cats.Domain.Common.Enums;

public enum FeedbackType
{
    [Description("Advisory")]
    Advisory = 0,
    
    [Description("Accepted by Exception")]
    AcceptedByException = 1
}