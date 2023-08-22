namespace Qhta.WPF.Utils;

/// <summary>
/// Establishes synchronized binding between CollectionView and Collection which has thread-safe operations.
/// Defines many attached properties for Column and ItemsControls (like DataGrid) that help to format DataGridColumns.
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

  #region EnableCollectionSychronization property
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
  /// When is set to true, then establishes synchronized binding 
  /// between CollectionView and Collection which has thread-safe operations.
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
  /// Sets a method to handle SortingEventHandler property changed event.
  /// If this method <see cref="DataGridSortingEventHandler"/>, it is assigned to the data grid.
  /// Otherwise a default handler is assigned which redirects event to DataContext implementing IListViewModel interface.
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

  #region IsSelectable property

  /// <summary>
  /// Specifies whether rows of the collection can be separately selected.
  /// </summary>
  public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.RegisterAttached(
      "IsSelectable",
      typeof(object),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(default(object), OnIsSelectableChanged));

  /// <summary>
  /// Getter for IsSelectable property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static object GetIsSelectable(DependencyObject target)
  {
    return (object)target.GetValue(IsSelectableProperty);
  }

  /// <summary>
  /// Setter for IsSelectable property
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetIsSelectable(DependencyObject target, object value)
  {
    target.SetValue(IsSelectableProperty, value);
  }

  static void OnIsSelectableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var dataGrid = sender as DataGrid;
    if (dataGrid == null || e.NewValue == null)
      return;
    dataGrid.Dispatcher.Invoke((Action)(() =>
    {
      try
      {
        dataGrid.SelectedIndex=-1;
      }
      catch
      {
      }
    }));
  }

  #endregion IsSelectable

  #region ScrollIntoView property

  /// <summary>
  /// Getter for ScrollIntoView property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static object GetScrollIntoView(DependencyObject target)
  {
    return (object)target.GetValue(ScrollIntoViewProperty);
  }

  /// <summary>
  /// Setter for ScrollIntoView property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetScrollIntoView(DependencyObject target, object value)
  {
    target.SetValue(ScrollIntoViewProperty, value);
  }

  /// <summary>
  /// Helper property to store added row which is passed to DataGrid ScrollIntoView.
  /// </summary>
  public static readonly DependencyProperty ScrollIntoViewProperty = DependencyProperty.RegisterAttached(
      "ScrollIntoView",
      typeof(object),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(default(object), OnScrollIntoViewChanged));

  /// <summary>
  /// Handler for ScrollIntoView property changed event. Invoked DataGrid ScrollIntoView method.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private static void OnScrollIntoViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var dataGrid = sender as DataGrid;
    if (dataGrid == null || e.NewValue == null)
      return;
    dataGrid.Dispatcher.Invoke((Action)(() =>
    {
      try
      {
        dataGrid.UpdateLayout();
        dataGrid.ScrollIntoView(e.NewValue, null);
        dataGrid.UpdateLayout();
      }
      catch
      {
      }
    }));
  }
  #endregion

  #region HiddenHeader property

  /// <summary>
  /// Specifies a hidden header string for a column. 
  /// This header is not displayed, but may be used e.g. in filtering dialog.
  /// </summary>
  public static readonly DependencyProperty HiddenHeaderProperty = DependencyProperty.RegisterAttached(
      "HiddenHeader",
      typeof(string),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(null));

  /// <summary>
  /// Getter for HiddenHeader property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static string GetHiddenHeader(DependencyObject target)
  {
    return (string)target.GetValue(HiddenHeaderProperty);
  }

  /// <summary>
  /// Setter for HiddenHeader property
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetHiddenHeader(DependencyObject target, string value)
  {
    target.SetValue(HiddenHeaderProperty, value);
  }
  #endregion
}
