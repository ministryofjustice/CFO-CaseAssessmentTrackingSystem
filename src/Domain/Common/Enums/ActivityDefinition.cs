using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ActivityDefinition : SmartEnum<ActivityDefinition>
{
    public DeliveryLocationType DeliveryLocationType { get; private set; }
    public ActivityETEType ActivityETEType { get; private set; }
    public ClassificationType ClassificationType { get; private set; }
    public string ActivityTitle { get; private set; }
    public ExpectedClaims ExpectedClaims { get; private set; }
    public CheckType CheckType { get; private set; }

    public static IEnumerable<ActivityDefinition> GetForLocationType(LocationType locationType)
    {
        if (locationType is { IsCommunity: true, IsHub: false })
        {
            return List
                   .Where(ad => ad.DeliveryLocationType == DeliveryLocationType.WiderCommunity)
                   .ToList();
        }

        if (locationType.IsCustody)
        {
            return List
                   .Where(ad => ad.DeliveryLocationType == DeliveryLocationType.Custody)
                   .ToList();
        }

        if (locationType.IsHub)
        {
            return List
                   .Where(ad => ad.DeliveryLocationType == DeliveryLocationType.Hub)
                   .ToList();
        }

        return [];
    }

    private ActivityDefinition(
        string name,
        int value,
        DeliveryLocationType deliveryLocationType,
        ClassificationType classificationType,
        ActivityETEType activityType,
        string activityTitle,
        ExpectedClaims expectedClaims,
        CheckType checkType) 
        : base(name, value)
    {
        DeliveryLocationType = deliveryLocationType;
        ClassificationType = classificationType;
        ActivityETEType = activityType;
        ActivityTitle = activityTitle;
        ExpectedClaims = expectedClaims;
        CheckType = checkType;
    }

    public static readonly ActivityDefinition AccessingHealthSupportCustody = new(
        "Accessing Health Support Custody",
        1,
        DeliveryLocationType.Custody,
        ClassificationType.NonISWActivity,
        ActivityETEType.SupportWork,
        "Accessing Health Support",
        ExpectedClaims.MoreThanOne,
        CheckType.Dip
    );

    public static readonly ActivityDefinition AddictionCustody = new ActivityDefinition(
        "Addiction Custody",
        2,
        DeliveryLocationType.Custody,
        ClassificationType.NonISWActivity,
        ActivityETEType.SupportWork,
        "Addiction",
        ExpectedClaims.MoreThanOne,
        CheckType.Dip
    );

    public static readonly ActivityDefinition AngerManagementSupportCustody = new ActivityDefinition(
        "Anger Management Support Custody",
        3,
        DeliveryLocationType.Custody,
        ClassificationType.NonISWActivity,
        ActivityETEType.SupportWork,
        "Anger Management Support",
        ExpectedClaims.MoreThanOne,
        CheckType.Dip
    );

    public static readonly ActivityDefinition ApplicationsJobsCustody = new ActivityDefinition(
        "Applications (Jobs) Custody",
        4,
        DeliveryLocationType.Custody,
        ClassificationType.NonISWActivity,
        ActivityETEType.SupportWork,
        "Applications (Jobs)",
        ExpectedClaims.MoreThanOne,
        CheckType.Dip
    );

    public static readonly ActivityDefinition AccessingHealthSupportHub = new(
      "Accessing Health Support Hub",
      1,
      DeliveryLocationType.Hub,
      ClassificationType.NonISWActivity,
      ActivityETEType.SupportWork,
      "Accessing Health Support",
      ExpectedClaims.MoreThanOne,
      CheckType.Dip
  );
}

// Enums for related properties using Ardalis SmartEnum

public class DeliveryLocationType : SmartEnum<DeliveryLocationType>
{
    public static readonly DeliveryLocationType Custody = new DeliveryLocationType("Custody", 0);
    public static readonly DeliveryLocationType WiderCommunity = new DeliveryLocationType("Wider Community", 1);
    public static readonly DeliveryLocationType Hub = new DeliveryLocationType("Hub", 2);
    private DeliveryLocationType(string name, int value) : base(name, value) { }
}

public class ClassificationType : SmartEnum<ClassificationType>
{
    public static readonly ClassificationType EducationAndTraining = new ClassificationType("Education and Training", 0);
    public static readonly ClassificationType Employment = new ClassificationType("Employment", 1);
    public static readonly ClassificationType ISWActivity = new ClassificationType("ISW Activity", 2);
    public static readonly ClassificationType NonISWActivity = new ClassificationType("Non-ISW Activity", 3);

    private ClassificationType(string name, int value) : base(name, value) { }
}

public class ActivityETEType : SmartEnum<ActivityETEType>
{
    public static readonly ActivityETEType CommunityAndSocial = new ActivityETEType("Community and Social", 0);
    public static readonly ActivityETEType EducationAndTraining = new ActivityETEType("Education and Training", 1);
    public static readonly ActivityETEType Employment = new ActivityETEType("Employment", 2);
    public static readonly ActivityETEType HumanCitizenship = new ActivityETEType("Human Citizenship", 3);
    public static readonly ActivityETEType InterventionsAndServicesWraparoundSupport = new ActivityETEType("Interventions And Services Wraparound Support", 4);
    public static readonly ActivityETEType SupportWork = new ActivityETEType("Support Work", 5);

    private ActivityETEType(string name, int value) : base(name, value) { }
}

public class ExpectedClaims : SmartEnum<ExpectedClaims>
{
    public static readonly ExpectedClaims MoreThanOne = new ExpectedClaims("> 1", 0);
    public static readonly ExpectedClaims One = new ExpectedClaims("1", 1);

    private ExpectedClaims(string name, int value) : base(name, value) { }
}

public class CheckType : SmartEnum<CheckType>
{
    public static readonly CheckType Dip = new CheckType("Dip", 0);
    public static readonly CheckType QA = new CheckType("QA", 1);

    private CheckType(string name, int value) : base(name, value) { }
}