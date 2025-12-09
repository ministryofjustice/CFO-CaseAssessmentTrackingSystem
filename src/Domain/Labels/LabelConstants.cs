namespace Cfo.Cats.Domain.Labels;

public static class LabelConstants
{
    /// <summary>
    /// The minimum length the name should be for a label
    /// </summary>
    public const int NameMinimumLength = 2;
    
    /// <summary>
    /// The maximum length the name should be for a label
    /// </summary>
    public const int NameMaximumLength = 15;
    
    /// <summary>
    /// The minimum length a description should be
    /// </summary>
    public const int DescriptionMinimumLength = 3;
    
    /// <summary>
    /// The maximum length a description for a label should be.
    /// </summary>
    public const int DescriptionMaximumLength = 200;
}