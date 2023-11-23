using Qhta.WPF.Utils.ViewModels;
using Qhta.WPF.Utils.Views;

namespace Qhta.WPF.Utils;

public partial class CollectionViewBehavior
{

  #region ShowFilterButton property
  /// <summary>
  /// Getter of ShowFilterButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool? GetShowFilterButton(DependencyObject? target)
  {
    if (target == null) return null;
    if (target is DataGridColumnHeader header)
      return (bool)header.GetValue(ShowFilterButtonProperty);
    var result = target.GetValue(ShowFilterButtonProperty);
    return (bool?)result;
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
  /// Specifies whether a "Filter" button should be displayed in the column header.
  /// </summary>
  public static readonly DependencyProperty ShowFilterButtonProperty = DependencyProperty.RegisterAttached(
      "ShowFilterButton",
      typeof(bool?),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(null));
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
  /// Routed event to store FilterButtonClick event handler. It the DataGridColumnHeader
  /// has not handled this event, then <see cref="OnShowFilterButton_Clicked"/> is invoked.
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
  public virtual void OnShowFilterButton_Clicked(object sender, RoutedEventArgs args)
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
        var filteredCollection = itemsSource as IFilteredCollection;
        while (itemsSource is CollectionView collectionView)
          itemsSource = collectionView.SourceCollection;
        if (sourceCollectionView == null && filteredCollection == null)
        {
          sourceCollectionView = itemsControl.Items as CollectionView;
          if (sourceCollectionView == null)
          {
            MessageBox.Show(CommonStrings.CantFilterThisCollection);
            return;
          }
        }
        if (itemsSource != null)
        {
          var itemsSourceType = itemsSource.GetType();
          if (itemsSourceType.IsEnumerable(out var itemType))
          {
            var column = args.Source as DataGridColumn;
            if (column != null)
            {
              if (column.TryGetFilterablePropertyPath(itemType, out var propPath))
              {
                var propertyInfo = propPath.LastOrDefault();
                if (propertyInfo != null)
                {
                  var ok = DisplayFilterDialog(itemsControl, column, propPath, button.PointToScreen(new Point(button.ActualWidth, button.ActualHeight)),
                    VisualTreeHelperExt.FindAncestor<Window>(button));
                  if (ok)
                  {
                    var viewModel = CollectionViewBehavior.GetColumnFilter(column) as ColumnFilterViewModel;
                    if (viewModel != null)
                    {
                      var filter = viewModel.CreateFilter();
                      SetFilterButtonShape(column, filter != null ? FilterButtonShape.Filled : FilterButtonShape.Empty);
                      var collectionViewFilter = GetCollectionFilter(itemsControl) as CollectionViewFilter;
                      if (collectionViewFilter == null)
                      {
                        collectionViewFilter = new CollectionViewFilter();
                        SetCollectionFilter(itemsControl, collectionViewFilter);
                      }

                      collectionViewFilter = collectionViewFilter.ApplyFilter(propertyInfo.Name, filter);

                      if (sourceCollectionView != null)
                        sourceCollectionView.Filter = collectionViewFilter.GetPredicate();
                      else
                      if (filteredCollection != null)
                        filteredCollection.Filter = collectionViewFilter;
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

  /// <summary>
  /// Displays a filter dialog for a column bound to the specific property info path.
  /// Dialog is displayed in specific screen position.
  /// View model is stored in column's attached ColumnFilter property.
  /// </summary>
  /// <param name="itemsControl">Control with columns. Usually DataGrid</param>
  /// <param name="column"></param>
  /// <param name="propPath"></param>
  /// <param name="position"></param>
  /// <param name="ownerWindow"></param>
  public virtual bool DisplayFilterDialog(ItemsControl itemsControl, DataGridColumn column, PropPath propPath, Point position, Window? ownerWindow)
  {
    var propInfo = propPath.Last();
    var columnName = column.GetHeaderText();
    if (columnName == null)
    {
      columnName = GetHiddenHeader(column);
      if (columnName == null)
        columnName = propInfo.Name;
    }
    var dialog = new FilterDialog();
    var viewModel = new DataGridFilterViewModel();
    var filter = (GetColumnFilter(column) as ColumnFilterViewModel)?.CreateCopy();
    if (filter != null)
      filter = new GenericColumnFilterViewModel(filter, columnName);
    else
    {
      filter = new GenericColumnFilterViewModel(propPath, columnName);
      //if (!propInfo.PropertyType.IsNullable(out var propType))
      //  propType = propInfo.PropertyType;
      //  if (propType == typeof(string))
      //    filter = new TextFilterViewModel(propPath, propName);
      //  else
      //  if (propType == typeof(bool))
      //    filter = new BoolFilterViewModel(propPath, propName);
      //  else
      //  if (propType.IsEnum)
      //    filter = new EnumFilterViewModel(propType, propPath, propName);
      //  else
      //  if (propType == typeof(int))
      //    filter = new NumFilterViewModel<int>(propPath, propName);
      //  else
      //  if (propType == typeof(uint))
      //    filter = new NumFilterViewModel<uint>(propPath, propName);
      //  else
      //  if (propType == typeof(byte))
      //    filter = new NumFilterViewModel<byte>(propPath, propName);
      //  else
      //  if (propType == typeof(sbyte))
      //    filter = new NumFilterViewModel<sbyte>(propPath, propName);
      //  else
      //  if (propType == typeof(Int16))
      //    filter = new NumFilterViewModel<Int16>(propPath, propName);
      //  else
      //  if (propType == typeof(UInt16))
      //    filter = new NumFilterViewModel<UInt16>(propPath, propName);
      //  else
      //  if (propType == typeof(Int64))
      //    filter = new NumFilterViewModel<Int64>(propPath, propName);
      //  else
      //  if (propType == typeof(UInt64))
      //    filter = new NumFilterViewModel<UInt64>(propPath, propName);
      //  else
      //  if (propType == typeof(Single))
      //    filter = new NumFilterViewModel<Single>(propPath, propName);
      //  else
      //  if (propType == typeof(Double))
      //    filter = new NumFilterViewModel<Double>(propPath, propName);
      //  else
      //  if (propType == typeof(Decimal))
      //    filter = new NumFilterViewModel<Decimal>(propPath, propName);
      //  else
      //  if (propType == typeof(DateTime))
      //    filter = new NumFilterViewModel<DateTime>(propPath, propName);
      //  else
      //  if (propType == typeof(TimeSpan))
      //    filter = new NumFilterViewModel<TimeSpan>(propPath, propName);
      //  else
      //    filter = new ObjFilterViewModel(propPath, propName);
    }
    viewModel.Filter = filter;
    viewModel.Column = column;
    //viewModel.PropName = filter.PropName;
    if (itemsControl is DataGrid dataGrid)
    {
      viewModel.Columns = GetFilteredColumns(dataGrid);
      viewModel.Column = column;
    }
    dialog.DataContext = viewModel;
    dialog.Left = position.X;
    dialog.Top = position.Y;
    dialog.Owner = ownerWindow;
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
  /// Helper property which defines visual shape of the filter button. 
  /// The type of this property is FilterButtonShape. It can be "Empty" or "Filled".
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
  /// Helper property to store ColumnFilter property. It can be any object.
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
  /// Helper property to store CollectionFilter property. It can be any object.
  /// </summary>
  public static readonly DependencyProperty CollectionFilterProperty = DependencyProperty.RegisterAttached(
      "CollectionFilter",
      typeof(object),
      typeof(CollectionViewBehavior),
      new PropertyMetadata(null));
  #endregion


  /// <summary>
  /// Gets a collection of filterable columns of the data grid.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <returns></returns>
  public DataGridFilteredColumns? GetFilteredColumns(DataGrid dataGrid)
  {
    var itemsSource = dataGrid.ItemsSource;
    if (itemsSource == null)
      throw new InvalidOperationException($"ItemsSource property is null in DataGrid");
    var dataItemType = itemsSource.GetType();
    if (dataItemType == null)
      return null;
    if (dataItemType.IsGenericType)
      dataItemType = dataItemType.GetGenericArguments().FirstOrDefault();
    if (dataItemType == null)
      return null;
    if (dataItemType.IsArray)
      dataItemType = dataItemType.GetElementType();
    else if (dataItemType.IsEnumerable(out var itemType))
      dataItemType = itemType;
    if (dataItemType == null)
      return null;
    var columns = new DataGridFilteredColumns();
    foreach (var column in dataGrid.Columns)
      if (column is DataGridBoundColumn boundColumn)
      {
        if (column.TryGetFilterablePropertyPath(dataItemType, out var filterablePropertyPath))
          columns.Add(new DataGridFilteredColumnInfo(dataItemType, boundColumn));
      }
    return columns;
  }
}
