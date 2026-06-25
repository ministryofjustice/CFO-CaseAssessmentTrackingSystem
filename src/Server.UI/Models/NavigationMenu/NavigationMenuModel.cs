using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Models.NavigationMenu;

/// <summary>
/// Represents the entire navigation menu.
/// </summary>
/// <param name="Sections">A collection of menu items grouped by section</param>
public record NavigationMenuModel(NavigationMenuSectionModel[] Sections);

/// <summary>
/// Represents a collection of links grouped by a commonality
/// </summary>
/// <param name="Title">The heading for the title</param>
/// <param name="Links">The links that sit under this section</param>
public record NavigationMenuSectionModel(string Title, NavigationMenuItemModel[] Links);

/// <summary>
/// Represents the smallest deriviative of the main menu (for example links and buttons)
/// </summary>
public abstract record NavigationMenuItemModel();

/// <summary>
/// Represents a menu item that should be dispayed as a button.
/// </summary>
/// <param name="DisplayText">The text to display in the button</param>
/// <param name="Href">The Href to navigate to</param>
/// <param name="AccessibilityText">Accessibility description</param>
public sealed record NavigationMenuItemButtonModel(string DisplayText, string? Href, string AccessibilityText, AppColour Colour = AppColour.Default) 
    : NavigationMenuItemModel();

/// <summary>
/// Represents a menu item that should be display as a link.
/// </summary>
/// <param name="DisplayText">The text to display as the link</param>
/// <param name="Href">The URL of the link</param>
/// <param name="AccessbilityText">Accessibility description</param>
/// <param name="Target">HTML standard for target</param>
public sealed record NavigationMenuItemLinkModel(string DisplayText, string? Href, string AccessbilityText, string Target="self")
    : NavigationMenuItemModel();

/// <summary>
/// Represents a menu divider.
/// </summary>
public sealed record NavigationMenuItemDividerModel() 
    : NavigationMenuItemModel();
