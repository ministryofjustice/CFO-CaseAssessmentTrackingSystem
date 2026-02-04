namespace Cfo.Cats.Application.SecurityConstants;

public static class RoleNames
{
    public const string QAFinance = "QA + Finance";
    public const string QAOfficer = "CFO QA Officer";
    public const string QASupportManager = "CFO QA Support Manager";
    public const string QAManager = "CFO QA Manager";
    public const string PerformanceManager = "Performance Manager";
    public const string SMT = "SMT";
    public const string SystemSupport = "System Support";
    public const string Finance = "Finance";
    public const string CSO = "Contract Support Officer";
    public const string CPM = "Contract Performance Manager";
    public const string CMPSM = "Contract Management Process Support Manager";

    public static string[] AllExtraPermissions = [
        QAFinance,
        QAOfficer,
        QASupportManager,
        QAManager,
        PerformanceManager,
        SMT,
        SystemSupport,
        Finance,
        CSO,
        CPM,
        CMPSM
    ];

    /// <summary>
    /// Roles that only exist for internal staff.
    /// </summary>
    public static string[] InternalRoles =
    [
        QAOfficer,
        PerformanceManager,
        SMT,
        SystemSupport,
        CSO,
        CPM,
        CMPSM,
    ];

}
