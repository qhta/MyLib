﻿using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Qhta.MVVM;
using Qhta.ObservableObjects;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;


namespace Qhta.SF.Tools;

/// <summary>
/// Navigation bar for navigating through records in a <see cref="SfDataGrid"/>.
/// It provides commands to navigate to the first, previous, next, and last records.
/// It also displays the current record number and the total number of records in the grid.
/// It should be placed under the <see cref="SfDataGrid"/>.
/// </summary>
public partial class RecordNavigationBar : UserControl, INotifyPropertyChanged
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RecordNavigationBar"/> class.
  /// </summary>
  /// <remarks>This constructor sets up the navigation commands for moving between records. It initializes the
  /// component and assigns commands for navigating to the next, last, previous, and first items.</remarks>
  public RecordNavigationBar()
  {
    InitializeComponent();
    NextItemCommand = new RelayCommand(NextItemExecute);
    LastItemCommand = new RelayCommand(LastItemExecute);
    PreviousItemCommand = new RelayCommand(PreviousItemExecute);
    FirstItemCommand = new RelayCommand(FirstItemExecute);
  }

  /// <summary>
  /// Dependency property for the <see cref="SfDataGrid"/> associated with this navigation bar.
  /// </summary>
  public static DependencyProperty DataGridProperty =
    DependencyProperty.Register(nameof(DataGrid), typeof(SfDataGrid), typeof(RecordNavigationBar),
      new PropertyMetadata(null, DataGridPropertyChanged));

  private static void DataGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is RecordNavigationBar recordNavigationBar)
    {
      if (e.NewValue is SfDataGrid dataGrid)
      {
        recordNavigationBar.DataGridChanged(dataGrid);
      }
    }
  }

  /// <summary>
  /// Updates the binding of the <see cref="RowsCountProperty"/> to reflect the current item count of the specified data
  /// grid.
  /// </summary>
  /// <remarks>This method sets a binding on the <see cref="RowsCountProperty"/> to the item count of the
  /// provided data grid's data source. Ensure that the <paramref name="dataGrid"/> is not <see langword="null"/> before
  /// calling this method.</remarks>
  /// <param name="dataGrid">The <see cref="SfDataGrid"/> whose item count is used to update the binding. Cannot be <see langword="null"/>.</param>
  private void DataGridChanged(SfDataGrid dataGrid)
  {
    SetBinding(RowsCountProperty, new Binding("ItemsSource.Count") { Source = dataGrid });
    dataGrid.Loaded += (s, e) =>
    {
      //Debug.WriteLine($"DataGrid {dataGrid.Name} Loaded");
      BindRowsCount(dataGrid);
    };
  }


  private void BindRowsCount(SfDataGrid dataGrid)
  {
    if (dataGrid.ItemsSource is ILoadable loadable)
    {
      if (dataGrid.View != null)
      {
        //Debug.WriteLine($"DataGrid {dataGrid.Name} View is not null, binding to Records.Count.");
        //SetBinding(RowsCountProperty, new Binding("Records.Count") { Source = dataGrid.View, Mode=BindingMode.OneWay });
        //Debug.WriteLine($"DataGrid {dataGrid.Name} View is not null, binding to CollectionChanged event.");
        dataGrid.View.CollectionChanged += ViewOnCollectionChanged(dataGrid);
      }
      else
      {
        //Debug.WriteLine($"DataGrid {dataGrid.Name} View is null, binding to Loaded event.");
        loadable.Loaded += (object? sender, EventArgs e) =>
        {
          //Debug.WriteLine($"DataGrid {dataGrid.Name} View Loaded");
          if (dataGrid.View != null)
          {
            //Debug.WriteLine($"DataGrid {dataGrid.Name} View is not null, binding to CollectionChanged event.");
            dataGrid.View.CollectionChanged += ViewOnCollectionChanged(dataGrid);
          }
        };
      }
      if (dataGrid.View?.Records != null)
        RowsCount = dataGrid.View.Records.Count;
    }
  }


  private NotifyCollectionChangedEventHandler? ViewOnCollectionChanged(SfDataGrid dataGrid)
  {
    return (s, e) =>
    {
      //Debug.WriteLine($"DataGrid {dataGrid.Name} View CollectionChanged {e.Action}");
      RowsCount = dataGrid.View.Records.Count;
      //Debug.WriteLine($"RowsCount: {dataGrid.View.Records.Count}");
    };
  }

  /// <summary>
  /// Gets or sets the <see cref="SfDataGrid"/> instance associated with this component.
  /// </summary>
  public SfDataGrid? DataGrid
  {
    get => (SfDataGrid?)GetValue(DataGridProperty);
    set => SetValue(DataGridProperty, value);
  }

  /// <summary>
  /// Dependency property for the number of rows in the <see cref="SfDataGrid"/>.
  /// </summary>
  public static DependencyProperty RowsCountProperty =
    DependencyProperty.Register(nameof(RowsCount), typeof(int), typeof(RecordNavigationBar),
      new PropertyMetadata(null));

  /// <summary>
  /// Gets or sets the number of rows in the collection.
  /// </summary>
  public int RowsCount
  {
    get => (int)GetValue(RowsCountProperty);
    set => SetValue(RowsCountProperty, value);
  }

  #region NextItemCommand
  /// <summary>
  /// Command to navigate to the next item in the <see cref="SfDataGrid"/>.
  /// </summary>
  public RelayCommand NextItemCommand { get; set; }

  /// <summary>
  /// Selects the next item in the data grid, if available.
  /// </summary>
  /// <remarks>This method increments the selected index of the data grid by one, provided that the data grid is
  /// not null and the current selection is not the last item. If the selection is successfully updated, the new item is
  /// scrolled into view.</remarks>
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
  #endregion

  #region LastItemCommand
  /// <summary>
  /// Command to navigate to the last item in the <see cref="SfDataGrid"/>.
  /// </summary>
  public RelayCommand LastItemCommand { get; set; }

  /// <summary>
  /// Selects the last item in the data grid and scrolls it into view.  
  /// </summary>
  /// <remarks>This method sets the selected index of the data grid to the last item and ensures it is visible
  /// by scrolling it into view. If the data grid is not initialized, the method exits without making changes.</remarks>
  public void LastItemExecute()
  {
    var dataGrid = DataGrid;
    if (dataGrid == null)
      return;

    var selectedIndex = RowsCount - 1;

    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }
  #endregion

  #region PreviousItemCommand
  /// <summary>
  /// Command to navigate to the previous item in the <see cref="SfDataGrid"/>.
  /// </summary>
  public RelayCommand PreviousItemCommand { get; set; }

  /// <summary>
  /// Selects the previous item in the data grid, if possible.
  /// </summary>
  /// <remarks>This method decreases the selected index of the data grid by one, if the current selection is not
  /// already at the first item. If the data grid is null or the first item is selected, the method does
  /// nothing.</remarks>
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
  #endregion

  #region FirstItemCommand
  /// <summary>
  /// Command to navigate to the first item in the <see cref="SfDataGrid"/>.
  /// </summary>
  public RelayCommand FirstItemCommand { get; set; }

  /// <summary>
  /// Selects and executes the first item in the data grid.
  /// </summary>
  /// <remarks>This method sets the selected index of the data grid to the first item and ensures it is scrolled
  /// into view. If the data grid is not initialized, the method exits without performing any action.</remarks>
  public void FirstItemExecute()
  {
    var dataGrid = DataGrid;
    if (dataGrid == null)
      return;

    var selectedIndex = 0;

    dataGrid.SelectedIndex = selectedIndex;
    ScrollInView(selectedIndex);
  }
  #endregion

  /// <summary>
  /// This method scrolls the specified row into view in the <see cref="SfDataGrid"/>.
  /// </summary>
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

  /// <summary>
  /// Occurs when a property value changes.
  /// </summary>
  /// <remarks>This event is typically used to notify clients, such as user interfaces, that a property value
  /// has changed.</remarks>
  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// Notifies subscribers that a property value has changed.
  /// </summary>
  /// <remarks>This method raises the <see cref="PropertyChanged"/> event, which is used to notify listeners,
  /// typically UI elements, that a property value has changed.</remarks>
  /// <param name="propertyName">The name of the property that changed. This value is optional and can be automatically supplied by the compiler.</param>
  protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

}