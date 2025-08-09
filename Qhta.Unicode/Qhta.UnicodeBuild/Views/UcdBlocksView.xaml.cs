using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Views;

/// <summary>
/// View for displaying Unicode character blocks in a data grid.
/// </summary>
public partial class UcdBlocksView : UserControl
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UcdBlocksView"/> class.
  /// </summary>
  public UcdBlocksView()
  {
    InitializeComponent();
    NextItemCommand = new RelayCommand<SfDataGrid>(NextItemExecute);
    LastItemCommand = new RelayCommand<SfDataGrid>(LastItemExecute);
    PreviousItemCommand = new RelayCommand<SfDataGrid>(PreviousItemExecute);
    FirstItemCommand = new RelayCommand<SfDataGrid>(FirstItemExecute);
  }


  private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    RowHeightProvider.OnQueryRowHeight(sender, e);
    if (e.Handled)
      return;
    LongTextColumn.OnQueryRowHeight(sender, e);
  }

  /// <summary>
  /// Command to navigate to the next item in the data grid.
  /// </summary>
  public RelayCommand<SfDataGrid> NextItemCommand { [DebuggerStepThrough] get; set; }

  private void NextItemExecute(SfDataGrid? dataGrid)
  {
    if (dataGrid == null) return;

    var collection = dataGrid.ItemsSource as UcdBlocksCollection;
    if (collection == null) return;

    var selectedIndex = dataGrid.SelectedIndex;
    if (selectedIndex >= collection.Count - 1)
      return;

    selectedIndex++;
    dataGrid.SelectedIndex = selectedIndex;
    var rowColumnIndex = new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(selectedIndex+1, 0);

    if (!dataGrid.IsLoaded)
    {
      dataGrid.Loaded += (s, e) => dataGrid.ScrollInView(rowColumnIndex);
    }
    else
    {
      dataGrid.ScrollInView(rowColumnIndex);
    }
    var selectedItem = dataGrid.View.Records[selectedIndex].Data;
    dataGrid.View.MoveCurrentTo(selectedItem);
  }

  /// <summary>
  /// Command to navigate to the last item in the data grid.
  /// </summary>
  public RelayCommand<SfDataGrid> LastItemCommand { [DebuggerStepThrough] get; set; }

  private void LastItemExecute(SfDataGrid? dataGrid)
  {
    if (dataGrid == null) return;

    var collection = dataGrid.ItemsSource as UcdBlocksCollection;
    if (collection == null) return;

    var selectedIndex = collection.Count - 1;

    dataGrid.SelectedIndex = selectedIndex;
    var rowColumnIndex = new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(selectedIndex + 1, 0);

    if (!dataGrid.IsLoaded)
    {
      dataGrid.Loaded += (s, e) => dataGrid.ScrollInView(rowColumnIndex);
    }
    else
    {
      dataGrid.ScrollInView(rowColumnIndex);
    }
    var selectedItem = dataGrid.View.Records[selectedIndex].Data;
    dataGrid.View.MoveCurrentTo(selectedItem);
  }

  /// <summary>
  /// Command to navigate to the previous item in the data grid.
  /// </summary>
  public RelayCommand<SfDataGrid> PreviousItemCommand { [DebuggerStepThrough] get; set; }

  private void PreviousItemExecute(SfDataGrid? dataGrid)
  {
    if (dataGrid == null) return;

    var collection = dataGrid.ItemsSource as UcdBlocksCollection;
    if (collection == null) return;

    var selectedIndex = dataGrid.SelectedIndex;
    if (selectedIndex <= 0)
      return;

    selectedIndex--;
    dataGrid.SelectedIndex = selectedIndex;
    var rowColumnIndex = new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(selectedIndex+1, 0);

    if (!dataGrid.IsLoaded)
    {
      dataGrid.Loaded += (s, e) => dataGrid.ScrollInView(rowColumnIndex);
    }
    else
    {
      dataGrid.ScrollInView(rowColumnIndex);
    }
    var selectedItem = dataGrid.View.Records[selectedIndex].Data;
    dataGrid.View.MoveCurrentTo(selectedItem);
  }

  /// <summary>
  /// Command to navigate to the first item in the data grid.
  /// </summary>
  public RelayCommand<SfDataGrid> FirstItemCommand { [DebuggerStepThrough] get; set; }

  private void FirstItemExecute(SfDataGrid? dataGrid)
  {
    if (dataGrid == null) return;

    var collection = dataGrid.ItemsSource as UcdBlocksCollection;
    if (collection == null) return;

    var selectedIndex = 0;

    dataGrid.SelectedIndex = selectedIndex;
    var rowColumnIndex = new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(selectedIndex + 1, 0);

    if (!dataGrid.IsLoaded)
    {
      dataGrid.Loaded += (s, e) => dataGrid.ScrollInView(rowColumnIndex);
    }
    else
    {
      dataGrid.ScrollInView(rowColumnIndex);
    }
    var selectedItem = dataGrid.View.Records[selectedIndex].Data;
    dataGrid.View.MoveCurrentTo(selectedItem);
  }

  /// <summary>
  /// Command to fill the current column in the data grid with a selected value.
  /// </summary>
  public static RoutedCommand FillColumnCommand { [DebuggerStepThrough] get; } = new RoutedCommand();

  private bool FillColumnCommandCanExecute(object? sender)
  {
    Debug.WriteLine($"FillColumnCommandCanExecute({sender})");
    if (sender is SfDataGrid dataGrid)
    {
      var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns,
        out var allRowsSelected, out var selectedRows);
      if (allColumnsSelected || selectedColumns.Length != 1)
      {
        return false;
      }
      var column = selectedColumns.FirstOrDefault();
      if (column != null)
      {
        var firstItem = selectedRows.FirstOrDefault();
        if (firstItem == null)
        {
          return false;
        }
        return true;
      }
    }
    return false;
  }

  private void FillColumnCommandExecute(object? sender)
  {
    if (sender is SfDataGrid dataGrid)
    {
      var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
      if (allColumnsSelected || selectedColumns.Length != 1)
      {
        Debug.WriteLine("FillColumnCommand: No column selected or multiple columns selected.");
        return;
      }
      var column = selectedColumns.FirstOrDefault();
      if (column != null)
      {
        var firstItem = selectedRows.FirstOrDefault();
        if (firstItem == null)
        {
          Debug.WriteLine("FillColumnCommand: No rows selected.");
          return;
        }
        if (column is GridComboBoxColumn comboBoxColumn)
        {
          var mappingName = comboBoxColumn.MappingName;
          var property = firstItem.GetType().GetProperty(mappingName);
          if (property == null) return;
          var propertyType = property.PropertyType;
          var itemsSource = comboBoxColumn.ItemsSource;
          var selectValueWindow = new SelectValueWindow
          {
            Prompt = String.Format(UnicodeBuild.Resources.Strings.SelectValueForField, column.HeaderText),
            ItemsSource = itemsSource,
            ShowOverwriteNonEmptyCells = true,
          };
          if (selectValueWindow.ShowDialog() == true)
          {
            var selectedValue = selectValueWindow.SelectedItem;
            var overwriteNonEmptyCells = selectValueWindow.OverwriteNonEmptyCells;
            if (selectedValue != null)
            {
              //Debug.WriteLine($"Setting column: {mappingName}, Selected Value: {selectedValue}");
              foreach (var record in selectedRows)
              {
                if (overwriteNonEmptyCells)
                {
                  property.SetValue(record, selectedValue);
                }
                else
                {
                  var currentValue = property.GetValue(record);
                  if (currentValue == null)
                    property.SetValue(record, selectedValue);
                }
              }
            }
          }
        }
      }
    }
  }

}

