using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;

namespace Qhta.UnicodeBuild.Views;
/// <summary>
/// Interaction logic for CodePointsView.xaml
/// </summary>
public partial class UcdCodePointsView : UserControl
{
  public UcdCodePointsView()
  {
    InitializeComponent();
  }


  private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    if (sender is SfDataGrid dataGrid && e.RowIndex > 0 && e.RowIndex <= dataGrid.View.Records.Count)
    {
      LongTextColumn.OnQueryRowHeight(sender, e);
      var rowIndex = e.RowIndex - 1;
      var rowData = dataGrid.View.Records[rowIndex].Data as UcdCodePointViewModel;
      var glyphSize = (rowData?.GlyphSize ?? 12);
      var rowHeight = (glyphSize * 200) / 100;
      if (rowHeight > e.Height)
      {
        e.Height = rowHeight;
        e.Handled = true;
        Debug.WriteLine($"Row {rowIndex} height = {rowHeight}");
      }
    }

  }

  private void UpDown_OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is UpDown UpDown)
    {
      var dataGrid = UpDown.FindAscendant<SfDataGrid>();
      if (dataGrid == null)
        return;
      var rowData = UpDown.DataContext as UcdCodePointViewModel;
      var collection = dataGrid.ItemsSource as UcdCodePointsCollection;
      if (collection == null || rowData == null)
        return;
      var rowIndex = collection.IndexOf(rowData);
      if (rowIndex < 0)
        return;
      var column = dataGrid.Columns.FirstOrDefault(item => item.MappingName == "Glyph");
      if (column == null)
        return;
      var glyphSize = (rowData?.GlyphSize ?? 12);
      var colWidth = glyphSize - 12 + 34;
      if (colWidth > column.Width)
      {
        column.Width = colWidth;
        //Debug.WriteLine($"Column {column.MappingName} width = {colWidth}");
      }
      dataGrid.View.Refresh();
    }
  }

  private void CodePointDataGrid_OnFilterItemsPopulating(object? sender, GridFilterItemsPopulatingEventArgs e)
  {
    //Debug.WriteLine($"Filtering {e.Column.MappingName}");
    if (e.Column.MappingName == "UcdBlock")
    {
      e.FilterControl.FilterMode = FilterMode.CheckboxFilter;
      var filterControlTemplate = e.FilterControl.Template;
      if (filterControlTemplate != null)
      {
        // Find the element by its Name within the template
        if (filterControlTemplate.FindName("PART_CheckboxFilterControl", e.FilterControl) is CheckboxFilterControl checkboxFilterControl)
        {
          //Debug.WriteLine("Found CheckboxFilterControl in the template.");
          var selectableNames = _ViewModels.Instance.UcdBlocks.Select(item => item.BlockName).ToArray();
          checkboxFilterControl.ItemsSource = selectableNames;
          e.ItemsSource = selectableNames.Select(name => new FilterElement { ActualValue = name }).ToArray();
          // Perform operations on the checkboxFilterControl
          e.Handled = true;
        }
      }
    }
    else
    if (e.Column.MappingName == "UcdRangeName")
    {
      e.FilterControl.FilterMode = FilterMode.CheckboxFilter;
      var filterControlTemplate = e.FilterControl.Template;
      if (filterControlTemplate != null)
      {
        // Find the element by its Name within the template
        if (filterControlTemplate.FindName("PART_CheckboxFilterControl", e.FilterControl) is CheckboxFilterControl checkboxFilterControl)
        {
          //Debug.WriteLine("Found CheckboxFilterControl in the template.");
          var selectableNames = _ViewModels.Instance.UcdRanges.Select(item => item.RangeName).ToArray();
          checkboxFilterControl.ItemsSource = selectableNames;
          e.ItemsSource = selectableNames.Select(name => new FilterElement { ActualValue = name }).ToArray();
          // Perform operations on the checkboxFilterControl
          e.Handled = true;
        }
      }
    }
  }


}

//public class CustomFilter : IGridFilter
//{
//  public Predicate<object> Filter { get; private set; }

//  public void ApplyFilter(GridColumn column, FilterPredicate filterPredicate, FilterType filterType)
//  {
//    if (filterPredicate == null || filterPredicate.FilterValue == null)
//    {
//      Filter = null; return;
//    } 
//    string filterValue = filterPredicate.FilterValue.ToString(); 
//    Filter = new Predicate<object>(item => 
//    { 
//      var propertyValue = column.GetValue(item); 
//      return propertyValue != null && propertyValue.ToString().Contains(filterValue);
//    });
//  }
//  public void ClearFilter() { Filter = null; }
//}

