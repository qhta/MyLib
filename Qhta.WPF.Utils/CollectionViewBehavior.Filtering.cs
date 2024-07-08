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
    bool? result = null;
    if (target is DataGridColumnHeader header)
      result = (bool?)header.GetValue(ShowFilterButtonProperty);
    else
    if (target != null)
      result = (bool?)target.GetValue(ShowFilterButtonProperty);
    Debug.WriteLine($"GetShowFilterButton({target}, {result})");
    return (bool?)result;
  }

  /// <summary>
  /// Setter of ShowFilterButton property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetShowFilterButton(DependencyObject target, bool value)
  {
    Debug.WriteLine($"SetShowFilterButton({target}, {value})");
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
  public static void OnShowFilterButton_Clicked(object sender, RoutedEventArgs args)
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
          sourceCollectionView = itemsControl.Items as CollectionView;
        if (sourceCollectionView == null)
        {
          MessageBox.Show(CommonStrings.CantFilterThisCollection);
          return;
        }
        if (itemsSource != null)
        {
          var itemsSourceType = itemsSource.GetType();
          if (itemsSourceType.IsEnumerable(out var itemType))
          {
            var column = args.Source as DataGridColumn;
            if (column != null)
            {
              var columnInfo = GetFilterableColumnInfo(itemType, column);
              if (columnInfo != null)
              {
                var ok = DisplayFilterDialog(itemsControl, sourceCollectionView, columnInfo, button.PointToScreen(new Point(button.ActualWidth, button.ActualHeight)),
                  VisualTreeHelperExt.FindAncestor<Window>(button));
                args.Handled = true;
                //if (ok)
                //{
                //var viewModel = CollectionViewBehavior.GetCollectionFilter(itemsControl) as CollectionViewFilterViewModel;
                //if (viewModel != null)
                //{
                //  var filterableColumns = viewModel.Columns;
                //  var filteredColumns = viewModel.GetFilteredColumns();
                //  if (filterableColumns!=null && filteredColumns != null)
                //  MarkFilteredColumns(filterableColumns, filteredColumns);

                //  var filter = viewModel.CreateFilter();
                //  SetFilterButtonShape(column, filter != null ? FilterButtonShape.Filled : FilterButtonShape.Empty);
                //  var collectionViewFilter = GetCollectionFilter(itemsControl) as CollectionViewFilter;
                //  if (collectionViewFilter == null)
                //  {
                //    collectionViewFilter = new CollectionViewFilter();
                //    SetCollectionFilter(itemsControl, collectionViewFilter);
                //  }
                //  if (collectionViewFilter != null)
                //    collectionViewFilter = collectionViewFilter.ApplyFilter(propertyInfo.Name, filter);

                //  if (sourceCollectionView != null)
                //    sourceCollectionView.Filter = collectionViewFilter?.GetPredicate();
                //  else
                //  if (filteredCollection != null)
                //    filteredCollection.Filter = collectionViewFilter;
                //}
                //}
                //}
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
  /// <param name="collectionView">Filtered collections</param>
  /// <param name="column"></param>
  /// <param name="position"></param>
  /// <param name="ownerWindow"></param>
  public static bool DisplayFilterDialog(ItemsControl itemsControl, CollectionView collectionView,
    ColumnViewInfo column, Point position, Window? ownerWindow)
  {
    var propPath = column.PropPath;
    Debug.Assert(propPath != null);
    var propInfo = propPath.Last();
    Debug.Assert(propInfo != null);
    var columnName = column.ColumnName;
    Debug.Assert(columnName != null);
    var dialog = new FilterDialog();
    var viewModel = GetCollectionFilter(itemsControl) as CollectionViewFilterViewModel;
    FilterViewModel? previousFilter = viewModel?.CreateCopy();
    if (viewModel == null)
      viewModel = new CollectionViewFilterViewModel();
    FilterViewModel? editedInstance = viewModel.EditedInstance;

    viewModel.TargetControl = itemsControl;
    viewModel.CollectionView = collectionView;
    viewModel.Columns = GetFilterableColumns(itemsControl);
    viewModel.Column = column;//GetFilteredColumn(viewModel.Columns, column)
    var columnFilter = GetColumnFilter(column) as FilterViewModel;
    if (previousFilter != null)
    {
      if (!previousFilter.ContainsColumn(column))
      {
        var compoundFilter = previousFilter as CompoundFilterViewModel;
        if (compoundFilter == null)
        {
          compoundFilter = new CompoundFilterViewModel(viewModel) { Operation = BooleanOperation.And };
          if (editedInstance != null)
            compoundFilter.Add(editedInstance);
        }
        if (columnFilter == null)
          columnFilter = new GenericColumnFilterViewModel(column, viewModel);
        compoundFilter.Add(columnFilter);
        columnFilter = compoundFilter;
      }
    }
    else
    {
      if (columnFilter == null)
        columnFilter = new GenericColumnFilterViewModel(column, viewModel);
    }
    if (columnFilter!=null)
      viewModel.EditedInstance = columnFilter;
    dialog.DataContext = viewModel;
    dialog.Left = position.X;
    dialog.Top = position.Y;
    dialog.Owner = ownerWindow;
    bool result = false;
    if (dialog.ShowDialog() == true)
    {
      SetCollectionFilter(itemsControl, viewModel);
      result = true;
    }
    else
      viewModel.EditedInstance = previousFilter;
    var filterableColumns = viewModel.Columns;
    var filteredColumns = viewModel.GetFilteredColumns();
    if (filterableColumns != null)
      MarkFilteredColumns(filterableColumns, filteredColumns);
    collectionView.Filter = viewModel.CreateFilter()?.GetPredicate();
    SetColumnFilter(column, columnFilter);
    return result;
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
      new PropertyMetadata(null, CollectionFilterPropertyChanged));
  #endregion

  /// <summary>
  /// Method invoked on CollectionFilterProperty changed event.
  /// </summary>
  /// <param name="dependencyObject"></param>
  /// <param name="args"></param>
  public static void CollectionFilterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
  {
  }

  /// <summary>
  /// Gets a collection of filterable columns of the data grid.
  /// </summary>
  public static ColumnsViewInfo? GetFilterableColumns(ItemsControl itemsControl)
  {
    var columns = new ColumnsViewInfo();
    if (itemsControl is DataGrid dataGrid)
    {
      var dataItemType = GetDataItemType(dataGrid);
      if (dataItemType == null)
        return null;
      foreach (var column in dataGrid.Columns)
        {
          var info = GetFilterableColumnInfo(dataItemType, column);
          if (info != null)
            columns.Add(info);
        }
    }
    return columns;
  }

  /// <summary>
  /// Gets a dataItem type from DataGrid ItemsSource;
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public static Type? GetDataItemType(DataGrid dataGrid)
  {
    var itemsSource = dataGrid.ItemsSource;
    if (itemsSource == null)
      throw new InvalidOperationException($"ItemsSource property is null in DataGrid");
    if (itemsSource is ListCollectionView listCollectionView)
      itemsSource = listCollectionView.SourceCollection;
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
    return dataItemType;
  }

  /// <summary>
  /// Gets or creates a filterable column info of the data grid bound column.
  /// </summary>
  public static ColumnViewInfo? GetFilterableColumnInfo(Type dataItemType, DataGridColumn column)
  {
    if (column.TryGetFilterablePropertyPath(dataItemType, out var filterablePropertyPath))
    {
      var columnViewInfo = GetColumnViewInfo(column);
      if (columnViewInfo == null)
        columnViewInfo = new ColumnViewInfo();
      if (String.IsNullOrEmpty(columnViewInfo.ColumnName))
      {
        var columnName = column.GetHeaderText();
        if (String.IsNullOrEmpty(columnViewInfo.ColumnName))
          return null;
        columnViewInfo.ColumnName = columnName;
      }
      if (columnViewInfo.Column == null)
        columnViewInfo.Column = column;
      if (columnViewInfo.DataType == null)
        columnViewInfo.DataType = dataItemType;
      if (columnViewInfo.PropPath == null)
        columnViewInfo.PropPath = filterablePropertyPath;
      SetColumnViewInfo(column, columnViewInfo);
      return columnViewInfo;
    }

    //if (column.TryGetFilterablePropertyPath(dataItemType, out var filterablePropertyPath))
    //{
    //  var columnName = column.GetHeaderText();
    //  if (columnName == null)
    //    columnName = CollectionViewBehavior.GetHiddenHeader(column);
    //  if (columnName == null)
    //    columnName = "?";

    //  return new ColumnViewInfo{
    //    PropPath = filterablePropertyPath, 
    //    ColumnName = columnName, 
    //    DataType=dataItemType, 
    //    Column=column };
    //}
    return null;
  }

  /// <summary>
  /// Marks filtered columns with FilterButtonShape.Filled image 
  /// and all other filterableColumns with FilterButtonShape.Empty image.
  /// </summary>
  /// <param name="filterableColumns"></param>
  /// <param name="filteredColumns"></param>
  public static void MarkFilteredColumns(ColumnsViewInfo filterableColumns, ColumnsViewInfo? filteredColumns)
  {
    foreach (var columnInfo in filterableColumns)
    {
      var column = columnInfo.Column as DataGridColumn;
      if (column != null)
      {
        bool isColumnFiltered = filteredColumns?.Any(item => item.Column == column) == true;
        SetFilterButtonShape(column, isColumnFiltered ? FilterButtonShape.Filled : FilterButtonShape.Empty);
      }
    }
  }
}
