using System.Collections.ObjectModel;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Headered collection of menu items.
/// </summary>
public class MenuItemCollection: ObservableCollection<MenuItemObject>
{
  /// <summary>
  /// Header for the collection of menu items.
  /// </summary>
  public string? Header { get; set; }
}