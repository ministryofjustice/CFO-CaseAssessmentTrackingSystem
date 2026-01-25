namespace Cfo.Cats.Domain.Common.Enums;

/// <summary>
/// Allows the domain to store UI based metadata without reference to the UI packages.
/// </summary>

public enum AppIcon
{
    None = 0,

    // Person / Participant
    Person,
    Group,

    // Tagging / Classification
    Tag,
    Label,

    // Priority / Importance
    Star,

    // Status / Flags
    Flag,
    Info,
    Check,
    Alert,

    // Contextual / Organisational
    Calendar,
    Shield,
    Work,
    School,
    Home,
    Lock,

    /// <summary>
    /// Existing app icons from the assessment
    /// </summary>
    Enrolment,
    Objective,
    Task,
    Activity,
    Payment,
    Location,
    Reassignment,
    HubInduction,
    WingInduction,
    WingInductionPhase,
}

