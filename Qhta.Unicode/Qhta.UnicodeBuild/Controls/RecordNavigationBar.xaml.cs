using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using PropertyTools.Wpf;

using Qhta.MVVM;
using Qhta.UnicodeBuild.ViewModels;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;


namespace Qhta.UnicodeBuild.Controls;

public partial class RecordNavigationBar : UserControl, INotifyPropertyChanged
{
  public RecordNavigationBar()
  {
    InitializeComponent();
    NextItemCommand = new RelayCommand(NextItemExecute);
    LastItemCommand = new RelayCommand(LastItemExecute);
    PreviousItemCommand = new RelayCommand(PreviousItemExecute);
    FirstItemCommand = new RelayCommand(FirstItemExecute);
  }

  public static DependencyProperty DataGridProperty =
    DependencyProperty.Register(nameof(DataGrid), typeof(SfDataGrid), typeof(RecordNavigationBar),
      new PropertyMetadata(null, DataGridPropertyChanged));

  private static void DataGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is RecordNavigationBar recordNavigationBar)
    {
      if (e.NewValue is SfDataGrid dataGrid)
      {
        dataGrid.Loaded += (s, e) =>
        {
          recordNavigationBar.NotifyPropertyChanged(nameof(RowsCount));
        };
      }
    }
  }

  public SfDataGrid? DataGrid
  {
    get => (SfDataGrid?)GetValue(DataGridProperty);
    set => SetValue(DataGridProperty, value);
  }

  public int RowsCount
  => DataGrid?.View?.Records.Count ?? 0;
  //{
  //  get
  //  {
  //    if (DataGrid == null)
  //      return 0;
  //    if (DataGrid.EnableDataVirtualization)
  //    {
  //      QueryableCollectionView?
  //        //ICollectionViewAdv? 
  //          view = DataGrid?.View as QueryableCollectionView;
  //      return view?.ViewSource.Count() ?? 0; // For virtualized data

  //    }
  //    else
  //    {
  //      if (DataGrid.ItemsSource is ICollectionView collectionView)
  //      {
  //        return collectionView.Cast<Object>().Count(); // For ICollectionView
  //      }
  //      else if (DataGrid.ItemsSource is IEnumerable<object> enumerable)
  //      {
  //        return enumerable.Count(); // For IEnumerable
  //      }
  //      else if (DataGrid.ItemsSource is IList list)
  //      {
  //        return list.Count; // For IList
  //      }
  //    }
  //    return 0; // Default if no valid data source is found
  //  }
  //}

  public RelayCommand NextItemCommand { get; set; }

  public void NextItemExecute()
  {
    var dataGrid = DataGrid;
    if (dataGrid == null)
      return;

    var selectedIndex = dataGrid.SelectedIndex;
    if (selectedIndex >= RowsCount - 1)
      return;

    selectedIndex++;
    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }

  public RelayCommand LastItemCommand { get; set; }

  public void LastItemExecute()
  {
    var dataGrid = DataGrid;
    if (dataGrid == null)
      return;
    
    var selectedIndex = RowsCount - 1;

    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }

  public RelayCommand PreviousItemCommand { get; set; }

  public void PreviousItemExecute()
  {
    var dataGrid = DataGrid;
    if (dataGrid == null)
      return;

    var selectedIndex = dataGrid.SelectedIndex;
    if (selectedIndex <= 0)
      return;

    selectedIndex--;
    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }

  public RelayCommand FirstItemCommand { get; set; }

  public void FirstItemExecute()
  {
    var dataGrid = DataGrid;
    if (dataGrid == null)
      return;

    var selectedIndex = 0;

    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }

  private void ScrollInView(int selectedIndex)
  {
    //Debug.WriteLine($"selectedIndex = {selectedIndex}"); 
    var dataGrid = DataGrid;
    if (dataGrid == null)
      return;

    var rowColumnIndex = new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(selectedIndex + 1, 0);

    if (!dataGrid.IsLoaded)
    {
      dataGrid.Loaded += (s, e) => dataGrid.ScrollInView(rowColumnIndex);
    }
    else
    {
      //Debug.WriteLine($"ScrollInView = {rowColumnIndex.RowIndex}");
      dataGrid.ScrollInView(rowColumnIndex);
    }
    var selectedItem = dataGrid.View.Records[selectedIndex].Data;
    dataGrid.View.MoveCurrentTo(selectedItem);

    //var firstLine = dataGrid.GetVisualContainer().ScrollRows.GetVisibleLines().FirstOrDefault(line => line.Region == ScrollAxisRegion.Body);
    //var lastLine = dataGrid.GetVisualContainer().ScrollRows.GetVisibleLines().LastOrDefault(line => line.Region == ScrollAxisRegion.Body);
    //Debug.WriteLine($"firstLine={firstLine}");
    //Debug.WriteLine($"lastLine={lastLine}");
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
  {
    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    field = value;
    NotifyPropertyChanged(propertyName);
    return true;
  }
}