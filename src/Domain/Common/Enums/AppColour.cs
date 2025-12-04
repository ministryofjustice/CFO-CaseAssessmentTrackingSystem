namespace Cfo.Cats.Domain.Common.Enums;

/// <summary>
/// Allows the domain to store UI based metadata without reference to the UI packages.
///
/// Translated in higher layers to match the UI framework specific version
/// </summary>
public enum AppColour
{
    Default,
    Primary,
    Secondary,
    Info,
    Success,
    Warning,
    Error,
    Dark,
}