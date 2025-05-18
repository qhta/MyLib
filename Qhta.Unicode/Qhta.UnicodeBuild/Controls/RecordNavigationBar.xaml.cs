using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using PropertyTools.Wpf;

using Qhta.MVVM;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;

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

  public SfDataGrid DataGrid
  {
    get => (SfDataGrid)GetValue(DataGridProperty);
    set => SetValue(DataGridProperty, value);
  }

  public int RowsCount => DataGrid?.View?.Records.Count ?? 0;

  public RelayCommand NextItemCommand { get; set; }

  public void NextItemExecute()
  {
    var dataGrid = DataGrid;

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

    var selectedIndex = RowsCount - 1;

    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }

  public RelayCommand PreviousItemCommand { get; set; }

  public void PreviousItemExecute()
  {
    var dataGrid = DataGrid;

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

    var selectedIndex = 0;

    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }

  private void ScrollInView(int selectedIndex)
  {
    //Debug.WriteLine($"selectedIndex = {selectedIndex}"); 
    var dataGrid = DataGrid;
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