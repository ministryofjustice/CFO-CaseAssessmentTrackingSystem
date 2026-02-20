using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Labels;

public class LabelScope : SmartEnum<LabelScope>
{
    public static readonly LabelScope User = new(nameof(User), 0, true);
    public static readonly LabelScope System = new(nameof(System), 1, false);

    private LabelScope(string name, int value, bool allowUserCreation) : base(name, value) 
        => AllowUserCreation = allowUserCreation;

    public bool AllowUserCreation { get;}

}