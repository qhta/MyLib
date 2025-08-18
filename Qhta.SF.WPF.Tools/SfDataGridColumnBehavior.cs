using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// A behavior class for managing the selection state of a <see cref="GridColumn"/> in a Syncfusion DataGrid.
/// </summary>
public class SfDataGridColumnBehavior : Behavior<GridColumn>
{
  #region IsSelected Attached Property
  /// <summary>
  /// Gets whether the column is selected.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetIsSelected(DependencyObject obj)
  {
    var column = (obj is GridHeaderCellControl header) ? header.Column : obj as GridColumn;
    if (column == null)
      return false;
    var result = (bool)obj.GetValue(IsSelectedProperty);
    //Debug.WriteLine($"GetIsSelected({column.MappingName})={result}");
    return result;
  }

  /// <summary>
  /// Sets the selection state of the column.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetIsSelected(DependencyObject obj, bool value)
  {
    var column = (obj is GridHeaderCellControl header) ? header.Column : obj as GridColumn;
    if (column == null)
      return;
    //Debug.WriteLine($"SetIsSelected({column.MappingName})={value}");
    column.SetValue(IsSelectedProperty, value);
  }

  /// <summary>
  /// Attached dependency property that indicates whether the column is selected.
  /// </summary>
  public static readonly DependencyProperty IsSelectedProperty =
    DependencyProperty.RegisterAttached
    (
      "IsSelected", typeof(bool), typeof(SfDataGridColumnBehavior),
      new PropertyMetadata(false, OnIsSelectedChanged));

  /// <summary>
  /// Handles changes to the IsSelected property of a GridColumn.
  /// </summary>
  /// <remarks>This method updates the styles of the GridColumn based on its selection state. If the column is
  /// selected, it applies the "SelectedColumnHeaderStyle" and "SelectedGridStyle" resources. If not selected, it resets
  /// the header style to its default and applies the "UnselectedGridStyle" to the cell style.</remarks>
  /// <param name="d">The dependency object that represents the GridColumn whose IsSelected property has changed.</param>
  /// <param name="e">The event data that contains information about the property change, including the new value.</param>
  private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var column = d as GridColumn;
    if (column == null)
      return;
    var isSelected = (bool)e.NewValue;
    //Debug.WriteLine($"OnIsSelectedChanged({d})={isSelected}");
    if (isSelected)
    {
      column.HeaderStyle = Application.Current.FindResource("SelectedColumnHeaderStyle") as Style;
      column.CellStyle = Application.Current.FindResource("SelectedGridStyle") as Style;
    }
    else
    {
      // Reset to default style or template
      column.ClearValue(GridColumnBase.HeaderStyleProperty);
      column.CellStyle = Application.Current.FindResource("UnselectedGridStyle") as Style;
    }
  }
  #endregion

}

/// <summary>
/// Class containing extension methods for the <see cref="SfDataGridColumnBehavior"/>.
/// </summary>
public static class SfDataGridColumnBehaviorExtensions
{
  #region Finder Attached Property
  /// <summary>
  /// Gets the finder associated with the column.
  /// </summary>
  /// <param name="column"></param>
  /// <returns></returns>
  public static SfDataGridFinder? GetFinder(this GridColumn column)
  {
    var result = (SfDataGridFinder?)column.GetValue(FinderProperty);
    return result;
  }

  /// <summary>
  /// Sets the finder associated with the column.
  /// </summary>
  /// <param name="column"></param>
  /// <param name="value"></param>
  public static void SetFinder(this GridColumn column, SfDataGridFinder? value)
  {
    column.SetValue(FinderProperty, value);
  }

  /// <summary>
  /// Attached dependency property that holds the finder associated with the column.
  /// </summary>
  public static readonly DependencyProperty FinderProperty =
    DependencyProperty.RegisterAttached
    (
      "Finder", typeof(SfDataGridFinder), typeof(SfDataGridColumnBehavior),
      new PropertyMetadata(null));

  #endregion
}
