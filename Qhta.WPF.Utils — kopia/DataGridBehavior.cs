namespace Qhta.WPF.Utils;

/// <summary>
/// Collection of DataGrid behavior properties and events.
/// </summary>
public partial class DataGridBehavior
{
  #region UserCanFilter property
  /// <summary>
  /// Getter of UserCanFilter property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool GetUserCanFilter(DataGrid target)
  {
    return (bool)target.GetValue(UserCanFilterProperty);
  }

  /// <summary>
  /// Setter of UserCanFilter property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetUserCanFilter(DataGrid target, bool value)
  {
    target.SetValue(UserCanFilterProperty, value);
  }

  /// <summary>
  /// Dependency property to store UserCanFilter property.
  /// </summary>
  public static readonly DependencyProperty UserCanFilterProperty = DependencyProperty.RegisterAttached(
      "UserCanFilter",
      typeof(bool),
      typeof(DataGridBehavior),
      new PropertyMetadata(false));
  #endregion

  #region ShowFilterButton property
  /// <summary>
  /// Getter of ShowFilterButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool GetShowFilterButton(DataGrid target)
  {
    return (bool)target.GetValue(ShowFilterButtonProperty);
  }

  /// <summary>
  /// Setter of ShowFilterButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetShowFilterButton(DataGrid target, bool value)
  {
    target.SetValue(ShowFilterButtonProperty, value);
  }

  /// <summary>
  /// Dependency property to store ShowFilterButton property.
  /// </summary>
  public static readonly DependencyProperty ShowFilterButtonProperty = DependencyProperty.RegisterAttached(
      "ShowFilterButton",
      typeof(bool),
      typeof(DataGridBehavior),
      new PropertyMetadata(false));
  #endregion

  #region FilterButtonClick event

  /// <summary>
  /// Add accessor for ShowFilterButtonClick event.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void AddFilterButtonClickHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.AddHandler(DataGridBehavior.FilterButtonClickEvent, handler);
  }

  /// <summary>
  /// Remove accessor for FilterButtonClick event.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void RemoveFilterButtonClickHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.RemoveHandler(DataGridBehavior.FilterButtonClickEvent, handler);
  }

  /// <summary>
  /// Routed event to store FilterButtonClick event handler.
  /// </summary>
  public static readonly RoutedEvent FilterButtonClickEvent = EventManager.RegisterRoutedEvent
    ("FilterButtonClick",RoutingStrategy.Bubble,typeof(RoutedEventHandler), typeof(DataGridBehavior));

  /// <summary>
  /// Handler for Click event of ClearButton (defined in default ComboBox style).
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public void FilterButton_Click(object sender, RoutedEventArgs args)
  {
    if (sender is Button button)
    {
      var dataGridColumnHeader = VisualTreeHelperExt.FindAncestor<DataGridColumnHeader>(button);
      if (dataGridColumnHeader != null)
      {
        dataGridColumnHeader.RaiseEvent(new RoutedEventArgs(DataGridBehavior.FilterButtonClickEvent, dataGridColumnHeader.Column));
      }
    }
  }

  #endregion

  #region UserCanFind property
  /// <summary>
  /// Getter of UserCanFilter property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool GetUserCanFind(DataGrid target)
  {
    return (bool)target.GetValue(UserCanFindProperty);
  }

  /// <summary>
  /// Setter of UserCanFind property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetUserCanFind(DataGrid target, bool value)
  {
    target.SetValue(UserCanFindProperty, value);
  }

  /// <summary>
  /// Dependency property to store UserCanFind property.
  /// </summary>
  public static readonly DependencyProperty UserCanFindProperty = DependencyProperty.RegisterAttached(
      "UserCanFind",
      typeof(bool),
      typeof(DataGridBehavior),
      new PropertyMetadata(false));
  #endregion

  #region ShowFindButton property
  /// <summary>
  /// Getter of ShowFindButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool GetShowFindButton(DataGrid target)
  {
    return (bool)target.GetValue(ShowFindButtonProperty);
  }

  /// <summary>
  /// Setter of ShowFindButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetShowFindButton(DataGrid target, bool value)
  {
    target.SetValue(ShowFindButtonProperty, value);
  }

  /// <summary>
  /// Dependency property to store ShowFindButton property.
  /// </summary>
  public static readonly DependencyProperty ShowFindButtonProperty = DependencyProperty.RegisterAttached(
      "ShowFindButton",
      typeof(bool),
      typeof(DataGridBehavior),
      new PropertyMetadata(false));
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
      typeof(DataGridBehavior),
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
  //                                        typeof(DataGridBehavior),
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
  //                                        typeof(DataGridBehavior),
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
  //    typeof(DataGridBehavior),
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
  //    typeof(DataGridBehavior),
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

  //#endregion ScrollIntoView

}
