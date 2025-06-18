using System.Collections.ObjectModel;

namespace Qhta.UnicodeBuild.Helpers;

public class MenuItemCollection: ObservableCollection<MenuItemObject>
{
  public string? Header { get; set; }
}