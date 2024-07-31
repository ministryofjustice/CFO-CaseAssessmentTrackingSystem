using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class MappaCategory : SmartEnum<MappaCategory>
{
    public static readonly MappaCategory Unknown = new(nameof(Unknown), string.Empty, -1);
    public static readonly MappaCategory Category1 = new("Category 1", "Those subject to notification requirements under Sexual Offences Act 2003. These individuals are required to notify the police of their name, address and other personal details. The length of time an individual is required to register with the Police can be any period between 12 months and life, depending on their age, the age of the victim, the nature of the offence and the sentence received.", 1);
    public static readonly MappaCategory Category2 = new("Category 2", "Those convicted of mainly violent offences who have been sentenced to 12 months or more in custody or to detention in hospital and are now living in the community.", 2);
    public static readonly MappaCategory Category3 = new("Category 3", "Other dangerous individuals who have committed an offence in the past and are considered to pose a risk of serious harm to the public.", 3);
    public static readonly MappaCategory Category4 = new("Category 4", "Those subject to notification requirements under the Counter-Terrorism Act 2008; those convicted of terrorism offences who have been sentenced to 12 months or more in custody or to detention in hospital and are now living in the community; and those who have committed an offence and may be at risk of involvement in terrorism-related activity.", 4);

    private MappaCategory(string name, string description, int value) 
        : base(name, value) 
    {
        Description = description;
    }

    public string Description { get; private set; }
}
