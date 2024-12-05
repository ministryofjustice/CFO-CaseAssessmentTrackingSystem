using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ActivityDefinition : SmartEnum<ActivityDefinition>
{
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

    public DeliveryLocationType DeliveryLocationType { get; private set; }
    public ActivityETEType ActivityETEType { get; private set; }
    public ClassificationType ClassificationType { get; private set; }
    public string ActivityTitle { get; private set; }
    public ExpectedClaims ExpectedClaims { get; private set; }
    public CheckType CheckType { get; private set; }

    public static IEnumerable<ActivityDefinition> GetActivitiesForLocation(LocationType locationType)
    {
        DeliveryLocationType deliveryLocation = locationType switch
        {
            { IsCommunity: true, IsHub: false } => DeliveryLocationType.WiderCommunity,
            { IsCustody: true } => DeliveryLocationType.Custody,
            { IsHub: true } => DeliveryLocationType.Hub,
            _ => throw new InvalidOperationException("Unsupported delivery location")
        };

        return List.Where(ad => ad.DeliveryLocationType == deliveryLocation);
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

    public static readonly ActivityDefinition AFutureFocusHub = new(
  "A Future Focus Hub",
  1,
  DeliveryLocationType.Hub,
  ClassificationType.NonISWActivity,
  ActivityETEType.HumanCitizenship,
  "A Future Focus",
  ExpectedClaims.MoreThanOne,
  CheckType.Dip
);

    public static readonly ActivityDefinition AccomSuppAdviceHub = new(
"Accommodation Support / Advice Hub",
1,
DeliveryLocationType.Hub,
ClassificationType.NonISWActivity,
ActivityETEType.CommunityAndSocial,
"Accommodation Support / Advice",
ExpectedClaims.MoreThanOne,
CheckType.Dip
);

    public static readonly ActivityDefinition ApprovedPremisesWiderComm = new(
"Approved Premises Wider Community",
1,
DeliveryLocationType.WiderCommunity,
ClassificationType.ISWActivity,
ActivityETEType.InterventionsAndServicesWraparoundSupport,
"Approved Premises",
ExpectedClaims.One,
CheckType.QA
);

    public static readonly ActivityDefinition ApprovedPremisesHub = new(
"Approved Premises Hub",
1,
DeliveryLocationType.Hub,
ClassificationType.ISWActivity,
ActivityETEType.InterventionsAndServicesWraparoundSupport,
"Approved Premises",
ExpectedClaims.One,
CheckType.QA
);

    public static readonly ActivityDefinition EducationAndTrainingHub = new(
"Education and Training Hub",
1,
DeliveryLocationType.Hub,
ClassificationType.EducationAndTraining,
ActivityETEType.EducationAndTraining,
"Education and Training",
ExpectedClaims.MoreThanOne,
CheckType.QA
);

    public static readonly ActivityDefinition EducationAndTraininWiderComm = new(
"Education and Training Wider Community",
1,
DeliveryLocationType.WiderCommunity,
ClassificationType.EducationAndTraining,
ActivityETEType.EducationAndTraining,
"Education and Training",
ExpectedClaims.MoreThanOne,
CheckType.QA
);

    public static readonly ActivityDefinition EducationAndTraininWiderCust = new(
"Education and Training Wider Custody",
1,
DeliveryLocationType.Custody,
ClassificationType.EducationAndTraining,
ActivityETEType.EducationAndTraining,
"Education and Training",
ExpectedClaims.MoreThanOne,
CheckType.QA
);


    public static readonly ActivityDefinition EmploymentHub = new(
"Employment Hub",
1,
DeliveryLocationType.Hub,
ClassificationType.Employment,
ActivityETEType.Employment,
"Employment in Community",
ExpectedClaims.MoreThanOne,
CheckType.QA
);

    public static readonly ActivityDefinition EmploymentWiderComm = new(
"Employment Wider Community",
1,
DeliveryLocationType.WiderCommunity,
ClassificationType.Employment,
ActivityETEType.Employment,
"Employment in Community",
ExpectedClaims.MoreThanOne,
CheckType.QA
);

    public static readonly ActivityDefinition EmploymentCust = new(
"Employment on ROTL",
1,
DeliveryLocationType.Custody,
ClassificationType.Employment,
ActivityETEType.Employment,
"Employment on ROTL",
ExpectedClaims.MoreThanOne,
CheckType.QA
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