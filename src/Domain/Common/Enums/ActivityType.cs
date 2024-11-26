using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ActivityType(
    string name, 
    int value) : SmartEnum<ActivityType>(name, value)
{
    public static readonly ActivityType CommunityAndSocial = new("Community and Social", 0);
    public static readonly ActivityType EducationAndTraining = new("Education and Training", 1);
    public static readonly ActivityType Employment = new("Employment", 2);
    public static readonly ActivityType HumanAndCitizenship = new("Human and Citizenship", 3);
    public static readonly ActivityType InterventionsAndServicesWraparoundSupport = new("Interventions And Services Wraparound Support", 4);
    public static readonly ActivityType SupportWork = new("Support Work", 5);
}
