using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Labels;

public class LabelId : TypedIdValueBase
{
    public LabelId(Guid value) 
        : base(value)
    {
    }
}
