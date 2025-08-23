using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// A behavior class for managing the state of a Syncfusion DataGrid.
/// </summary>
public class SfDataGridBehavior : Behavior<SfDataGrid>
{
  #region AllowRowResizing property
  /// <summary>
  /// Gets whether the grid allows to resize rows by dragging a bottom edge of row-header cell.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetAllowRowResizing(DependencyObject obj)
  {
    var result = (bool)obj.GetValue(AllowRowResizingProperty);
    //Debug.WriteLine($"GetAllowRowResizing({column.MappingName})={result}");
    return result;
  }

  /// <summary>
  /// Sets whether the grid allows to resize rows by dragging a bottom edge of row-header cell.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetAllowRowResizing(DependencyObject obj, bool value)
  {
    //Debug.WriteLine($"SetAllowRowResizing({column.MappingName})={value}");
    obj.SetValue(AllowRowResizingProperty, value);
  }

  /// <summary>
  /// Attached dependency property that indicates whether the grid allows to resize rows by dragging a bottom edge of row-header cell.
  /// </summary>
  public static readonly DependencyProperty AllowRowResizingProperty =
    DependencyProperty.RegisterAttached
    (
      "AllowRowResizing", typeof(bool), typeof(SfDataGridBehavior),
      new PropertyMetadata(false));
  #endregion

  #region IsRowResizing property
  /// <summary>
  /// Gets whether the grid is now resizing rows by dragging a bottom edge of row-header cell.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetIsRowResizing(DependencyObject obj)
  {
    var result = (bool)obj.GetValue(IsRowResizingProperty);
    //Debug.WriteLine($"GetIsRowResizing({column.MappingName})={result}");
    return result;
  }

  /// <summary>
  /// Sets whether the grid is now resizing rows by dragging a bottom edge of row-header cell.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetIsRowResizing(DependencyObject obj, bool value)
  {
    //Debug.WriteLine($"SetIsRowResizing({column.MappingName})={value}");
    obj.SetValue(IsRowResizingProperty, value);
  }

  /// <summary>
  /// Attached dependency property that indicates whether the grid is now resizing rows by dragging a bottom edge of row-header cell.
  /// </summary>
  public static readonly DependencyProperty IsRowResizingProperty =
    DependencyProperty.RegisterAttached
    (
      "IsRowResizing", typeof(bool), typeof(SfDataGridBehavior),
      new PropertyMetadata(false));
  #endregion

  #region StartOffset property
  /// <summary>
  /// Gets the start offset for resizing rows in the grid. This is the difference between actual row height and the initial position of the mouse when resizing starts.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static double GetStartOffset(DependencyObject obj)
  {
    var result = (double)obj.GetValue(StartOffsetProperty);
    //Debug.WriteLine($"GetStartOffset({column.MappingName})={result}");
    return result;
  }

  /// <summary>
  /// Sets the start offset for resizing rows in the grid. This is the difference between actual row height and the initial position of the mouse when resizing starts.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetStartOffset(DependencyObject obj, double value)
  {
    //Debug.WriteLine($"SetStartOffset({column.MappingName})={value}");
    obj.SetValue(StartOffsetProperty, value);
  }

  /// <summary>
  /// Attached dependency property for the start offset for resizing rows in the grid. This is the difference between actual row height and the initial position of the mouse when resizing starts.
  /// </summary>
  public static readonly DependencyProperty StartOffsetProperty =
    DependencyProperty.RegisterAttached
    (
      "StartOffset", typeof(double), typeof(SfDataGridBehavior),
      new PropertyMetadata(0.0));
  #endregion

  #region AllowColumnManagement property
  /// <summary>
  /// Gets whether the grid allows to manage columns by <see cref="ColumnManagementCommand"/>.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetAllowColumnManagement(DependencyObject obj)
  {
    var result = (bool)obj.GetValue(AllowColumnManagementProperty);
    //Debug.WriteLine($"GetAllowColumnManagement({column.MappingName})={result}");
    return result;
  }

  /// <summary>
  /// Sets whether the grid allows to manage columns by <see cref="ColumnManagementCommand"/>.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetAllowColumnManagement(DependencyObject obj, bool value)
  {
    //Debug.WriteLine($"SetAllowColumnManagement({column.MappingName})={value}");
    obj.SetValue(AllowColumnManagementProperty, value);
  }

  /// <summary>
  /// Attached dependency property that indicates whether the grid allows to manage columns by <see cref="ColumnManagementCommand"/>.
  /// </summary>
  public static readonly DependencyProperty AllowColumnManagementProperty =
    DependencyProperty.RegisterAttached
    (
      "AllowColumnManagement", typeof(bool), typeof(SfDataGridBehavior),
      new PropertyMetadata(false));
  #endregion
}