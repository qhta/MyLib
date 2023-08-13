namespace Qhta.WPF.Utils;

/// <summary>
/// Interface that defines selecting methods.
/// </summary>
public interface IListSelector
{
  /// <summary>
  /// Select an item.
  /// </summary>
  /// <param name="item"></param>
  /// <param name="select"></param>
  void SelectItem(object item, bool select);

  /// <summary>
  /// Select all items.
  /// </summary>
  /// <param name="select"></param>
  void SelectAll(bool select);

  /// <summary>
  /// Count of selected items.
  /// </summary>
  int SelectedItemsCount { get; }

  /// <summary>
  /// Enumeration of selected items.
  /// </summary>
  IEnumerable<object> SelectedItems { get; }

  /// <summary>
  /// A method to notify that selection was changed.
  /// </summary>
  void NotifySelectionChanged();
}
