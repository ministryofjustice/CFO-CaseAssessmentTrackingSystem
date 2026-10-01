using Cfo.Cats.Server.UI.Components.Shared.Help;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cfo.Cats.Server.UI.Services.Help;

/// <summary>
/// Substitutes <see cref="HelpAwareMudTabs"/> for every <see cref="MudTabs"/> instantiated
/// anywhere in the app, so that the "current tab" portion of the help system works without
/// requiring any existing (or future) page to reference <see cref="HelpAwareMudTabs"/> directly.
/// </summary>
/// <remarks>
/// This is the app's only <see cref="IComponentActivator"/>. Any future feature that also needs
/// to intercept component creation should extend this class rather than registering a second
/// activator (only one can be active at a time).
///
/// <b>Maintenance note:</b> this depends on <c>MudBlazor.MudTabs</c> remaining a public,
/// non-sealed class with a public parameterless constructor. If a MudBlazor upgrade changes
/// that (compile error on <c>typeof(MudTabs)</c>/<c>new HelpAwareMudTabs()</c>, or tabs silently
/// stop reporting their active panel), check the MudBlazor release notes for <c>MudTabs</c>
/// changes first - this is the one place in the app relying on its exact shape.
/// </remarks>
public class HelpAwareComponentActivator(IServiceProvider serviceProvider) : IComponentActivator
{
    public IComponent CreateInstance(Type componentType)
    {
        if (componentType == typeof(MudTabs))
        {
            return new HelpAwareMudTabs();
        }

        // Some components (e.g. CaseAbout) use constructor injection rather than [Inject]
        // properties, so a plain Activator.CreateInstance would fail with "No parameterless
        // constructor defined". ActivatorUtilities.CreateInstance resolves constructor
        // dependencies from the app's service provider, matching what the framework's default
        // component activator does.
        return (IComponent)ActivatorUtilities.CreateInstance(serviceProvider, componentType);
    }
}
