using System.Windows;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Class representing a selectable column item in ColumnSelectionCollection.
/// </summary>
public class ColumnSelectionItem: DependencyObject, ISelectableItem
{
  /// <summary>
  /// Referenced GridColumn.
  /// </summary>
  public required GridColumn Column { get; set; }
  /// <summary>
  /// A displayed name of the column is its HeaderText.
  /// </summary>
  public string DisplayName => Column.HeaderText?.ToString() ?? string.Empty;
  /// <summary>
  /// Tooltip is null.
  /// </summary>
  public string? ToolTip => null;
  /// <summary>
  /// Actual value is the referenced GridColumn.
  /// </summary>
  public object? ActualValue => Column;

  /// <summary>
  /// Is selected checkbox state
  /// </summary>
  public bool IsSelected { get; set; }

  /// <summary>
  /// Default string representation is the DisplayName.
  /// </summary>
  public override string ToString() => DisplayName;
}