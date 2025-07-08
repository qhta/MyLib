using System.Collections;
using System.Diagnostics;
using System.Windows.Controls;

using Qhta.MVVM;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace Qhta.UnicodeBuild.Views;

/// <summary>
/// Interaction logic for UcdBlocksView.xaml
/// 
/// </summary>
public partial class UcdBlocksView : UserControl
{
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

  public RelayCommand<SfDataGrid> NextItemCommand { get; set; }

  public void NextItemExecute(SfDataGrid? dataGrid)
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

  public RelayCommand<SfDataGrid> LastItemCommand { get; set; }

  public void LastItemExecute(SfDataGrid? dataGrid)
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

  public RelayCommand<SfDataGrid> PreviousItemCommand { get; set; }

  public void PreviousItemExecute(SfDataGrid? dataGrid)
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

  public RelayCommand<SfDataGrid> FirstItemCommand { get; set; }

  public void FirstItemExecute(SfDataGrid? dataGrid)
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

