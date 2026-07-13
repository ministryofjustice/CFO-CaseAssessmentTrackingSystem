namespace Cfo.Cats.Server.UI.Services;

/// <summary>
/// Service to notify components when the user's workspace preference (HomePage) changes.
/// </summary>
public interface IWorkspacePreferenceService
{
    /// <summary>
    /// Event raised when the user's workspace preference changes.
    /// </summary>
    event Action? OnWorkspacePreferenceChanged;

    /// <summary>
    /// Notifies all listeners that the workspace preference has changed.
    /// </summary>
    void NotifyWorkspacePreferenceChanged();
}

public class WorkspacePreferenceService : IWorkspacePreferenceService
{
    public event Action? OnWorkspacePreferenceChanged;

    public void NotifyWorkspacePreferenceChanged() => OnWorkspacePreferenceChanged?.Invoke();
}
