using System.Windows.Controls;

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
  public RelayCommand<SfDataGrid> NextItemCommand { get; set; }

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
  public RelayCommand<SfDataGrid> LastItemCommand { get; set; }

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
  public RelayCommand<SfDataGrid> PreviousItemCommand { get; set; }

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
  public RelayCommand<SfDataGrid> FirstItemCommand { get; set; }

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


}

