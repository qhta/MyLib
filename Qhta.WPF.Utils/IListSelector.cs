using System.Collections.Generic;

namespace Qhta.WPF.Utils
{
  public interface IListSelector
  {
    void SelectItem(object item, bool select);

    void SelectAll(bool select);

    int SelectedItemsCount { get; }

    IEnumerable<object> SelectedItems { get; }

    void NotifySelectionChanged();
  }
}
