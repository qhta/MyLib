using Qhta.WPF.Utils.ViewModels;
using Qhta.WPF.Utils.Views;

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
    BindingOperations.CollectionViewRegistering += BindingOperations_CollectionViewRegistering;
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
        new UIPropertyMetadata(false, EnableCollectionSynchronizationPropertyChangedCallback));

  /// <summary>
  /// Callback method invoked on Items control.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="args"></param>
  private static void EnableCollectionSynchronizationPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    //Debug.WriteLine($"EnableCollectionSynchronizationPropertyChangedCallback({obj}, {args.NewValue})");
    if (obj is ItemsControl itemsControl)
    {
      itemsControl.Loaded += ItemsControl_Loaded;
    }
  }

  private static void BindingOperations_CollectionViewRegistering(object? sender, CollectionViewRegisteringEventArgs args)
  {
    //Debug.WriteLine($"CollectionViewRegistering({sender},{args.CollectionView})");
    CollectionView cv = args.CollectionView as CollectionView;
    if (cv != null)
    {
      //Debug.WriteLine($"AllowsCrossThreadChanges={cv.GetType().GetProperty("AllowsCrossThreadChanges", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cv)}");
    }
  }

  private static void BindingOperations_CollectionRegistering(object? sender, CollectionRegisteringEventArgs args)
  {
    //Debug.WriteLine($"CollectionRegistering({sender},{args.Collection})");
    if (args.Collection is IList itemsCollection)
      EnableIListCollectionSynchronization(itemsCollection, true);
    else if (args.Collection is IEnumerable enumerable)
      EnableIEnumerableCollectionSynchronization(enumerable, true);
  }

  private static void ItemsControl_Loaded(object sender, RoutedEventArgs e)
  {
    //Debug.WriteLine($"ItemsControl_Loaded({sender})");
    //if (sender is ItemsControl itemsControl)
    //{
    //  var enable = GetEnableCollectionSynchronization(itemsControl);
    //  EnableCollectionSynchronization(itemsControl, enable);
    //}
    //else
    //  Debug.WriteLine($"  {sender} is not an ItemsControl");
  }

  //private static void EnableCollectionSynchronization(ItemsControl itemsControl, bool enable)
  //{
  //  BindingExpression binding = itemsControl.GetBindingExpression(ItemsControl.ItemsSourceProperty);
  //  if (binding != null)
  //  {
  //    var source = binding.ResolvedSource;
  //    //Debug.WriteLine($"EnableCollectionSynchronization({itemsControl}, {source}, {enable})");
  //    if (source is IList collection)
  //    {
  //      EnableCollectionSynchronization(collection, enable);
  //    }
  //    //else
  //      //Debug.WriteLine($"Could not enable CollectionSynchronization. Source does not implement interface IList");
  //  }
  //  else
  //    Debug.WriteLine($"  ItemsSourceProperty binding is null");
  //}

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

  #region ShowFilterButton property
  /// <summary>
  /// Getter of ShowFilterButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool? GetShowFilterButton(DependencyObject target)
  {
    if (target is DataGridColumnHeader header)
      return (bool)header.GetValue(ShowFilterButtonProperty);
    return (bool)target.GetValue(ShowFilterButtonProperty);
  }

  /// <summary>
  /// Setter of ShowFilterButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetShowFilterButton(DependencyObject target, bool value)
  {
    target.SetValue(ShowFilterButtonProperty, value);
  }
  /// <summary>
  /// Dependency property to store ShowFilterButton property.
  /// </summary>
  public static readonly DependencyProperty ShowFilterButtonProperty = DependencyProperty.RegisterAttached(
      "ShowFilterButton",
      typeof(bool),
      typeof(CollectionViewBehavior),
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
      element.AddHandler(CollectionViewBehavior.FilterButtonClickEvent, handler);
  }

  /// <summary>
  /// Remove accessor for FilterButtonClick event.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void RemoveFilterButtonClickHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.RemoveHandler(CollectionViewBehavior.FilterButtonClickEvent, handler);
  }

  /// <summary>
  /// Routed event to store FilterButtonClick event handler.
  /// </summary>
  public static readonly RoutedEvent FilterButtonClickEvent = EventManager.RegisterRoutedEvent
    ("FilterButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CollectionViewBehavior));

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
        var newArgs = new RoutedEventArgs(CollectionViewBehavior.FilterButtonClickEvent, dataGridColumnHeader.Column);
        dataGridColumnHeader.RaiseEvent(newArgs);
        if (!newArgs.Handled)
          OnShowFilterButton_Clicked(sender, newArgs);
      }
    }
  }

  /// <summary>
  /// Default handler for ShowFilterButton_Clicked event.
  /// Invokes DisplayFilterDialog method.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  protected virtual void OnShowFilterButton_Clicked(object sender, RoutedEventArgs args)
  {
    if (sender is Button button)
    {
      var itemsControl = VisualTreeHelperExt.FindAncestor<ItemsControl>(button);
      if (itemsControl is DataGridColumnHeadersPresenter presenter)
        itemsControl = VisualTreeHelperExt.FindAncestor<ItemsControl>(presenter);
      if (itemsControl != null)
      {
        var itemsSource = itemsControl.ItemsSource;
        var sourceCollectionView = itemsSource as CollectionView;
        while (itemsSource is CollectionView collectionView)
          itemsSource = collectionView.SourceCollection;
        if (itemsSource != null)
        {
          var itemsSourceType = itemsSource.GetType();
          if (itemsSourceType.IsEnumerable(out var itemType))
          {
            var column = args.Source as DataGridColumn;
            if (column != null)
            {
              if (column is DataGridBoundColumn boundColumn)
              {
                var binding = boundColumn.Binding as Binding;
                if (binding != null)
                {
                  PropertyInfo? propertyInfo = null;
                  var path = binding.Path;
                  var valueType = itemType;
                  if (itemType != null && path != null)
                  {
                    var pathStrs = path.Path.Split('.');
                    foreach (var pathStr in pathStrs)
                    {
                      propertyInfo = valueType.GetProperty(pathStr);
                      if (propertyInfo != null)
                        valueType = propertyInfo.PropertyType;
                    }
                  }
                  if (valueType != null && propertyInfo != null)
                  {
                    var ok = DisplayFilterDialog(column, propertyInfo, button.PointToScreen(new Point(button.ActualWidth, button.ActualHeight)));
                    if (ok)
                    {
                      var viewModel = CollectionViewBehavior.GetColumnFilter(column) as ColumnFilterViewModel;
                      if (viewModel != null && propertyInfo != null)
                      {
                        var filter = viewModel.CreateFilter();
                        SetFilterButtonShape(column, filter != null ? FilterButtonShape.Filled : FilterButtonShape.Empty);
                        var collectionViewFilter = GetCollectionFilter(itemsControl) as CollectionViewFilter;
                        if (collectionViewFilter == null)
                        {
                          collectionViewFilter = new CollectionViewFilter();
                          SetCollectionFilter(itemsControl, collectionViewFilter);
                        }
                        if (sourceCollectionView != null)
                          sourceCollectionView.Filter = collectionViewFilter.ApplyFilter(propertyInfo.Name, filter);
                        else 
                        if (itemsSource is IFiltered filteredCollection)
                          filteredCollection.Filter = collectionViewFilter.ApplyFilter(propertyInfo.Name, filter);
                      }
                      args.Handled = true;
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }

  /// <summary>
  /// Displays a filter dialog for a column bound to the specific property.
  /// Dialog is displayed in specific screen position.
  /// View model is stored in column's attached ColumnFilter property.
  /// </summary>
  /// <param name="column"></param>
  /// <param name="propInfo"></param>
  /// <param name="position"></param>
  protected virtual bool DisplayFilterDialog(DataGridColumn column, PropertyInfo propInfo, Point position)
  {
    var propName = column.GetHeaderText() ?? propInfo.Name;
    var dialog = new ColumnFilterDialog();
    var viewModel = (GetColumnFilter(column) as ColumnFilterViewModel)?.CreateCopy();
    if (viewModel == null)
    {
      if (!propInfo.PropertyType.IsNullable(out var propType))
        propType = propInfo.PropertyType;
      if (propType == typeof(string))
        viewModel = new TextFilterViewModel(propInfo, propName);
      else
      if (propType == typeof(bool))
        viewModel = new BoolFilterViewModel(propInfo, propName);
    }
    dialog.DataContext = viewModel;
    dialog.Left = position.X;
    dialog.Top = position.Y;
    if (dialog.ShowDialog() == true)
    {
      SetColumnFilter(column, viewModel);
      return true;
    }
    return false;
  }
  #endregion


  #region FilterButtonShape property
  /// <summary>
  /// Getter of FilterButtonShape property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static FilterButtonShape? GetFilterButtonShape(DataGridColumn target)
  {
    return (FilterButtonShape)target.GetValue(FilterButtonShapeProperty);
  }

  /// <summary>
  /// Setter of FilterButtonShape property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetFilterButtonShape(DataGridColumn target, FilterButtonShape value)
  {
    target.SetValue(FilterButtonShapeProperty, value);
  }

  /// <summary>
  /// Dependency property to store FilterButtonShape property.
  /// </summary>
  public static readonly DependencyProperty FilterButtonShapeProperty = DependencyProperty.RegisterAttached(
      "FilterButtonShape",
      typeof(FilterButtonShape),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(FilterButtonShape.Empty));
  #endregion

  #region ColumnFilter property
  /// <summary>
  /// Getter of ColumnFilter property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static object? GetColumnFilter(DependencyObject target)
  {
    return (object?)target.GetValue(ColumnFilterProperty);
  }

  /// <summary>
  /// Setter of ColumnFilter property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetColumnFilter(DependencyObject target, object? value)
  {
    target.SetValue(ColumnFilterProperty, value);
  }

  /// <summary>
  /// Dependency property to store ColumnFilter property.
  /// </summary>
  public static readonly DependencyProperty ColumnFilterProperty = DependencyProperty.RegisterAttached(
      "ColumnFilter",
      typeof(object),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(null));
  #endregion

  #region CollectionFilter property
  /// <summary>
  /// Getter of CollectionFilter property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static object? GetCollectionFilter(DependencyObject target)
  {
    return (object?)target.GetValue(CollectionFilterProperty);
  }

  /// <summary>
  /// Setter of CollectionFilter property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetCollectionFilter(DependencyObject target, object? value)
  {
    target.SetValue(CollectionFilterProperty, value);
  }

  /// <summary>
  /// Dependency property to store CollectionFilter property.
  /// </summary>
  public static readonly DependencyProperty CollectionFilterProperty = DependencyProperty.RegisterAttached(
      "CollectionFilter",
      typeof(object),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(null));
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
