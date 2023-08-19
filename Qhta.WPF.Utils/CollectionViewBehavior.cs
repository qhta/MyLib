namespace Qhta.WPF.Utils;

/// <summary>
/// Defines <see cref="EnableCollectionSynchronizationProperty"/> to help establish synchronized binding 
/// between CollectionView and Collection which has thread-safe operations.
/// </summary>
public partial class CollectionViewBehavior
{
  /// <summary>
  /// Static constructor that initializes registering of collection and collection view.
  /// </summary>
  static CollectionViewBehavior()
  {
    BindingOperations.CollectionRegistering += BindingOperations_CollectionRegistering;
  }

  #region EnableCollectionSychronization
  /// <summary>
  /// Getter for EnableCollectionSynchronization property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetEnableCollectionSynchronization(DependencyObject obj)
  {
    return (bool)obj.GetValue(EnableCollectionSynchronizationProperty);
  }

  /// <summary>
  /// Setter for EnableCollectionSynchronization property.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetEnableCollectionSynchronization(DependencyObject obj, bool value)
  {
    obj.SetValue(EnableCollectionSynchronizationProperty, value);
  }

  /// <summary>
  /// Dependency property to store EnableCollectionSynchronization property.
  /// </summary>
  public static readonly DependencyProperty EnableCollectionSynchronizationProperty =
      DependencyProperty.RegisterAttached("EnableCollectionSynchronization", typeof(bool), typeof(CollectionViewBehavior),
        new UIPropertyMetadata(false));

  private static void BindingOperations_CollectionRegistering(object? sender, CollectionRegisteringEventArgs args)
  {
    //Debug.WriteLine($"CollectionRegistering({sender},{args.Collection})");
    if (args.Collection is IList itemsCollection)
      EnableIListCollectionSynchronization(itemsCollection, true);
    else if (args.Collection is IEnumerable enumerable)
      EnableIEnumerableCollectionSynchronization(enumerable, true);
  }


  private static void EnableIListCollectionSynchronization(IList itemsCollection, bool enable)
  {
    //Debug.WriteLine($"EnableIListCollectionSynchronization({itemsCollection},{enable})");
    if (enable && itemsCollection is INotifyCollectionChanged && itemsCollection.IsSynchronized)
    {
      //Debug.WriteLine($"  enable root={itemsCollection.SyncRoot}");
      BindingOperations.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot);
    }
    else
    {
      //Debug.WriteLine($"  disable");
      BindingOperations.DisableCollectionSynchronization(itemsCollection);
    }
  }

  private static void EnableIEnumerableCollectionSynchronization(IEnumerable itemsCollection, bool enable)
  {
    //Debug.WriteLine($"EnableIEnumerableCollectionSynchronization({itemsCollection},{enable})");
    if (enable && itemsCollection is INotifyCollectionChanged)
    {
      //Debug.WriteLine($"  enable");
      BindingOperations.EnableCollectionSynchronization(itemsCollection, itemsCollection);
    }
    else
    {
      //Debug.WriteLine($"  disable");
      BindingOperations.DisableCollectionSynchronization(itemsCollection);
    }
  }
  #endregion

  #region Sorting event

  /// <summary>
  /// Getter of SortingEventHandler property.
  /// We do not define new event, but a handler for already defined event.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static object GetSortingEventHandler(DependencyObject target)
  {
    return (object)target.GetValue(SortingEventHandlerProperty);
  }

  /// <summary>
  /// Setter of SortingEventHandler property.
  /// We do not define new event, but a handler for already defined event.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetSortingEventHandler(DependencyObject target, object value)
  {
    target.SetValue(SortingEventHandlerProperty, value);
  }

  /// <summary>
  /// Dependency property to store SortingEventHandler property.
  /// </summary>
  public static readonly DependencyProperty SortingEventHandlerProperty = DependencyProperty.RegisterAttached(
      "SortingEventHandler",
      typeof(object),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(default(object), OnSortingEventHandlerChanged));

  /// <summary>
  /// Method to handle SortingEventHandler property changed event.
  /// If its new value is a proper <see cref="DataGridSortingEventHandler"/>, it is assigned to the data grid.
  /// Otherwise a default handler is assigned.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private static void OnSortingEventHandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var dataGrid = sender as DataGrid;
    if (dataGrid == null || e.NewValue == null)
      return;
    dataGrid.Dispatcher.Invoke((Action)(() =>
    {
      if (e.NewValue is DataGridSortingEventHandler handler)
        dataGrid.Sorting += handler;
      else
        dataGrid.Sorting += DataGrid_Sorting;
    }));
  }

  /// <summary>
  /// Default handler method of the DataGrid sorting event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  private static void DataGrid_Sorting(object sender, DataGridSortingEventArgs args)
  {
    var dataGrid = sender as DataGrid;
    if (dataGrid != null)
    {
      if (dataGrid.DataContext is IListViewModel listViewModel)
        if (args.Column is DataGridTextColumn textColumn && textColumn.Binding is Binding binding)
        {
          List<string> sortedColumns = new List<string>();
          foreach (var column in dataGrid.Columns)
          {
            var sortDirection = column.SortDirection;
            if (sortDirection != null)
            {
              if (column == args.Column)
              {
                //swap sort direction
                if (sortDirection == ListSortDirection.Descending)
                  sortedColumns.Add($"{column.SortMemberPath}(asc)");
                else
                  sortedColumns.Add($"{column.SortMemberPath}(desc)");
              }
              else
              {
                // columns sorted previously
                if (sortDirection == ListSortDirection.Descending)
                  sortedColumns.Add($"{column.SortMemberPath}(desc)");
                else
                  sortedColumns.Add($"{column.SortMemberPath}(asc)");
              }
            }
          }
          var curColumn = args.Column;
          if (curColumn.SortDirection == null)
            sortedColumns.Add($"{curColumn.SortMemberPath}");
          listViewModel.SortedBy = string.Join(";", sortedColumns);
          //Debug.WriteLine($"SortedBy({listViewModel.SortedBy})");
        }
    }
  }

  #endregion SortingEventHandler


  //#region DisplayRowNumberOffset

  ///// <summary>
  ///// Sets the starting value of the row header if enabled
  ///// </summary>
  //public static DependencyProperty DisplayRowNumberOffsetProperty =
  //    DependencyProperty.RegisterAttached("DisplayRowNumberOffset",
  //                                        typeof(int),
  //                                        typeof(CollectionViewBehavior),
  //                                        new FrameworkPropertyMetadata(0, OnDisplayRowNumberOffsetChanged));

  //public static int GetDisplayRowNumberOffset(DependencyObject target)
  //{
  //  return (int)target.GetValue(DisplayRowNumberOffsetProperty);
  //}

  //public static void SetDisplayRowNumberOffset(DependencyObject target, int value)
  //{
  //  target.SetValue(DisplayRowNumberOffsetProperty, value);
  //}

  //private static void OnDisplayRowNumberOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
  //{
  //  DataGrid dataGrid = target as DataGrid;
  //  int offset = (int)e.NewValue;

  //  if (GetDisplayRowNumber(target))
  //  {
  //    GetVisualChildCollection<DataGridRow>(dataGrid).
  //            ForEach(d => d.Header = d.GetIndex() + offset);
  //  }
  //}

  //#endregion

  //#region DisplayRowNumber

  ///// <summary>
  ///// Enable display of row header automatically
  ///// </summary>
  ///// <remarks>
  ///// Source: 
  ///// </remarks>
  //public static DependencyProperty DisplayRowNumberProperty =
  //    DependencyProperty.RegisterAttached("DisplayRowNumber",
  //                                        typeof(bool),
  //                                        typeof(CollectionViewBehavior),
  //                                        new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));

  //public static bool GetDisplayRowNumber(DependencyObject target)
  //{
  //  return (bool)target.GetValue(DisplayRowNumberProperty);
  //}

  //public static void SetDisplayRowNumber(DependencyObject target, bool value)
  //{
  //  target.SetValue(DisplayRowNumberProperty, value);
  //}

  //private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
  //{
  //  DataGrid dataGrid = target as DataGrid;
  //  if ((bool)e.NewValue == true)
  //  {
  //    int offset = GetDisplayRowNumberOffset(target);

  //    EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
  //    loadedRowHandler = (object sender, DataGridRowEventArgs ea) =>
  //    {
  //      if (GetDisplayRowNumber(dataGrid) == false)
  //      {
  //        dataGrid.LoadingRow -= loadedRowHandler;
  //        return;
  //      }
  //      ea.Row.Header = ea.Row.GetIndex() + offset;
  //    };
  //    dataGrid.LoadingRow += loadedRowHandler;

  //    ItemsChangedEventHandler itemsChangedHandler = null;
  //    itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
  //    {
  //      if (GetDisplayRowNumber(dataGrid) == false)
  //      {
  //        dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
  //        return;
  //      }
  //      GetVisualChildCollection<DataGridRow>(dataGrid).
  //          ForEach(d => d.Header = d.GetIndex() + offset);
  //    };
  //    dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
  //  }
  //}
  //#endregion // DisplayRowNumber

  //#region IsSelectable
  //public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.RegisterAttached(
  //    "IsSelectable",
  //    typeof(object),
  //    typeof(CollectionViewBehavior),
  //    new PropertyMetadata(default(object), OnIsSelectableChanged));

  //public static object GetIsSelectable(DependencyObject target)
  //{
  //  return (object)target.GetValue(IsSelectableProperty);
  //}

  //public static void SetIsSelectable(DependencyObject target, object value)
  //{
  //  target.SetValue(IsSelectableProperty, value);
  //}

  //static void OnIsSelectableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  //{
  //  var dataGrid = sender as DataGrid;
  //  if (dataGrid == null || e.NewValue == null)
  //    return;
  //  dataGrid.Dispatcher.Invoke((Action)(() =>
  //  {
  //    try
  //    {
  //      //dataGrid.SelectedIndex=-1;
  //    }
  //    catch
  //    {
  //    }
  //  }));
  //}

  //#endregion IsSelectable

  //#region ScrollIntoView property

  ///// <summary>
  ///// Getter for ScrollIntoView property.
  ///// </summary>
  ///// <param name="target"></param>
  ///// <returns></returns>
  //public static object GetScrollIntoView(DependencyObject target)
  //{
  //  return (object)target.GetValue(ScrollIntoViewProperty);
  //}

  ///// <summary>
  ///// Setter for ScrollIntoView property.
  ///// </summary>
  ///// <param name="target"></param>
  ///// <param name="value"></param>
  //public static void SetScrollIntoView(DependencyObject target, object value)
  //{
  //  target.SetValue(ScrollIntoViewProperty, value);
  //}

  ///// <summary>
  ///// Dependency property to store ScrollIntoView property
  ///// </summary>
  //public static readonly DependencyProperty ScrollIntoViewProperty = DependencyProperty.RegisterAttached(
  //    "ScrollIntoView",
  //    typeof(object),
  //    typeof(CollectionViewBehavior),
  //    new PropertyMetadata(default(object), OnScrollIntoViewChanged));

  ///// <summary>
  ///// Handler for ScrollIntoView property changed event. Invoked DataGrid ScrollIntoView method.
  ///// </summary>
  ///// <param name="sender"></param>
  ///// <param name="e"></param>
  //private static void OnScrollIntoViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  //{
  //  var dataGrid = sender as DataGrid;
  //  if (dataGrid == null || e.NewValue == null)
  //    return;
  //  dataGrid.Dispatcher.Invoke((Action)(() =>
  //  {
  //    try
  //    {
  //      dataGrid.UpdateLayout();
  //      dataGrid.ScrollIntoView(e.NewValue, null);
  //      dataGrid.UpdateLayout();
  //    }
  //    catch
  //    {
  //    }
  //  }));
  //}

}
