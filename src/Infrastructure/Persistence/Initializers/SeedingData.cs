using System.Text.Json;
using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Identity;
using Polly;

namespace Cfo.Cats.Infrastructure.Persistence.Initializers;

public static class SeedingData
{

    public static IEnumerable<Tenant> GetTenants()
    {
        yield return Tenant.Create("1.", "CFO", "Root tenant for Creating Future Opportunities");
        yield return Tenant.Create("1.1.", "CFO Evolution", "Top level tenant for Evolution Programme");
        yield return Tenant.Create("1.1.1.", "Alpha", "Alpha");
        yield return Tenant.Create("1.1.2.", "Bravo", "Top level tenant for provider Bravo");
        yield return Tenant.Create("1.1.2.1.", "Bravo (A)", "Bravo (A)");
        yield return Tenant.Create("1.1.2.2.", "Bravo (B)", "Bravo (B)");
        yield return Tenant.Create("1.1.2.3.", "Bravo (C)", "Bravo (C)");
        yield return Tenant.Create("1.1.3.", "Charlie", "Charlie");
        yield return Tenant.Create("1.1.4.", "Delta", "Top level tenant for Delta");
        yield return Tenant.Create("1.1.4.1.", "Delta (A)", "Delta (A)");
        yield return Tenant.Create("1.1.4.2.", "Delta (B)", "Delta (B)");
        yield return Tenant.Create("1.1.5.", "Echo", "Echo");
        yield return Tenant.Create("1.1.6.", "Foxtrot", "Foxtrot");
    }

    public static IEnumerable<Contract> GetContracts()
    {
        yield return Contract.Create("Evolution1", 1, "North West", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution2", 2, "North East", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution3", 3, "Yorkshire and Humberside", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution4", 4, "West Midlands", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution5", 5, "East Midlands", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution6", 6, "East Of England", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution7", 7, "London", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution8", 8, "South West", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
        yield return Contract.Create("Evolution9", 9, "South East", "1.", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
    }

    public static IEnumerable<Location> GetLocations()
    {
       yield return Location.Create("Risley", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Lancaster", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Forest Bank", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Altcourse", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Preston", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Buckley Hall", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Liverpool", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Manchester", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Thorn Cross", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Haverigg", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Hindley", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Kirkham", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Wymott", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Styal", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution1", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Holme House", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution2", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Northumberland", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution2", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Durham", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution2", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Kirklevington Grange", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution2", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Low Newton", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution2", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Wealstun", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Moorland", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Humber", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Doncaster", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Leeds", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Hull", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Full Sutton", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Hatfield", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Lindholme", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Askham Grange", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("New Hall", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution3", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Featherstone", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Drake Hall", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Birmingham", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Brinsford", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Dovegate", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Hewell", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Oakwood", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Stoke Heath", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Swinfen Hall", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution4", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Ranby", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Nottingham", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Five Wells", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Lincoln", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("North Sea Camp", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Onley", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Stocken", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Whatton", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Foston Hall", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution5", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("The Mount", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Peterborough (M)", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Bedford", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Chelmsford", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Highpoint", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Hollesley Bay", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Littlehey", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Norwich", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Wayland", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Peterborough (F)", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution6", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("High Down", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Wandsworth", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Thameside", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Brixton", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Feltham", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Isis", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Pentonville", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Bronzefield", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Downview", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution7", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Portland", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Exeter", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Bristol", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Channings Wood", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Dartmoor", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Leyhill", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Guys Marsh", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("The Verne", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Eastwood Park", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution8", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Rochester", GenderProvision.FromName("Male"), LocationType.FromName("Wing"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Elmley", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Lewes", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Winchester", GenderProvision.FromName("Male"), LocationType.FromName("Feeder"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Aylesbury", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Bullingdon", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Ford", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Springhill", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Stanford Hill", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Swaleside", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Woodhill", GenderProvision.FromName("Male"), LocationType.FromName("Outlying"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("Send", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
       yield return Location.Create("East Sutton Park", GenderProvision.FromName("Female"), LocationType.FromName("Unspecified"), "Evolution9", new DateTime(2024, 5, 1), new DateTime(2029, 03, 31, 23, 59, 59));
    }

    public static IEnumerable<KeyValue> GetDictionaries()
    {
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "CFO Evolution Provider", Text = "CFO Evolution Provider", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Probation", Text = "Probation", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Approved Premises", Text = "Approved Premises", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "CAS2", Text = "CAS2", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "CAS3", Text = "CAS3", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Custodial Family Services", Text = "Custodial Family Services", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "CRS - Women", Text = "CRS- Women", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "CRS - Personal Wellbeing", Text = "CRS- Personal Wellbeing", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "CRS - Dependency & Recovery", Text = "CRS- Dependency & Recovery", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "CRS - Accommodation", Text = "CRS- Accommodation", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Custody staff", Text = "Custody staff", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "New Futures Network", Text = "New Futures Network", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Prison Education Provider", Text = "Prison Education Provider", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "DWP", Text = "DWP", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Healthcare", Text = "Healthcare", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Community / Voluntary Sector organisation", Text = "Community / Voluntary Sector organisation", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Local Authority", Text = "Local Authority", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Courts", Text = "Courts", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Self-referral", Text = "Self-referral", Description = "A referral source" };
        yield return new KeyValue { Name = Picklist.ReferralSource, Value = "Other", Text = "Other", Description = "A referral source (please state)" };

    }

}