using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

using PropertyTools.Wpf;

using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Data;
//using CollectionViewExtensions = Syncfusion.UI.Xaml.Grid.CollectionViewExtensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;

using CollectionViewExtensions = Syncfusion.Data.CollectionViewExtensions;

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
    if (e.Column.MappingName == "UcdBlock")
    {
      GridFilterControl filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Collapsed;
      filterControl.FilterMode = FilterMode.CheckboxFilter;
      var selectableItems = _ViewModels.Instance.UcdBlocks.OrderBy(item => item.Name).ToArray();

      UcdBlockFilters = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is UcdBlockViewModel val)
            return val.Name;
          return "";
        },
      }).ToArray();
      e.ItemsSource = UcdBlockFilters;
      e.Handled = true;
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

  private FilterElement[] UcdBlockFilters = null!;

  private void BlockFilterControl_OkButtonClick(object? sender, OkButtonClikEventArgs e)
  {
    if (sender is not GridFilterControl filterControl)
      return;
    var filterPredicates = UcdBlockColumn.FilterPredicates;
    filterPredicates.Clear();
    foreach (var filterElement in UcdBlockFilters)
    {
      if (filterElement.IsSelected)
      {
        filterPredicates.Add(new FilterPredicate
        {
          FilterValue = filterElement.ActualValue,
          FilterType = FilterType.Equals
        });
      }
    }
    if (CodePointDataGrid.View is CollectionViewAdv collectionView)
    {
      // Clear existing filters
      collectionView.Filter = null;

      // Define a new filter
      var filterBlock = _ViewModels.Instance.UcdBlocks.FirstOrDefault(item => item.Id == 1);
      collectionView.Filter = item =>
      {
        if (item is UcdCodePointViewModel codePoint)
        {
          // Example: Filter rows where GlyphSize is greater than 12
          return codePoint.UcdBlock?.Id == 1;
        }
        return false;
      };

      // Refresh the view to apply the filter
      collectionView.Refresh();
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

