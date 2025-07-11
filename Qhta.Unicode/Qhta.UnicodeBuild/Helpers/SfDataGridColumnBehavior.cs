using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Helpers;

public class SfDataGridColumnBehavior : Behavior<GridColumn>
{
  public static bool GetIsSelected(DependencyObject obj)
  {
    var column = (obj is GridHeaderCellControl header) ? header.Column : obj as GridColumn;
    if (column == null)
      return false;
    var result = (bool)obj.GetValue(IsSelectedProperty);
    //Debug.WriteLine($"GetIsSelected({column.MappingName})={result}");
    return result;
  }

  public static void SetIsSelected(DependencyObject obj, bool value)
  {
    var column = (obj is GridHeaderCellControl header) ? header.Column : obj as GridColumn;
    if (column == null)
      return;
    //Debug.WriteLine($"SetIsSelected({column.MappingName})={value}");
    column.SetValue(IsSelectedProperty, value);
  }

  public static readonly DependencyProperty IsSelectedProperty =
    DependencyProperty.RegisterAttached
    (
      "IsSelected", typeof(bool), typeof(SfDataGridColumnBehavior),
      new PropertyMetadata(false, OnIsSelectedChanged));


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
      //column.ClearValue(GridColumnBase.CellStyleProperty);
      column.CellStyle = Application.Current.FindResource("UnselectedGridStyle") as Style;
    }
  }

}