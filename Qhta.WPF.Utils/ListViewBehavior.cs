namespace Qhta.WPF.Utils;

/// <summary>
/// Behavior class that defines a few ListView properties.
/// </summary>
public static partial class ListViewBehavior
{

  #region FitLastColumnWidth property

  /// <summary>
  /// Getter for FitLastColumnWidth property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool GetFitLastColumnWidth(DependencyObject target)
  {
    return (bool)target.GetValue(FitLastColumnWidthProperty);
  }

  /// <summary>
  /// Setter for FitLastColumnWidth property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetFitLastColumnWidth(DependencyObject target, bool value)
  {
    target.SetValue(FitLastColumnWidthProperty, value);
  }

  /// <summary>
  /// Dependency property to store FitLastColumnWidth property.
  /// </summary>
  public static readonly DependencyProperty FitLastColumnWidthProperty = DependencyProperty.RegisterAttached(
      "FitLastColumnWidth",
      typeof(bool),
      typeof(ListViewBehavior),
      new PropertyMetadata(default(bool), OnFitLastColumnWidthChanged));

  private static void OnFitLastColumnWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var listView = sender as ListView;
    if (listView == null || e.NewValue == null)
      return;
    listView.Dispatcher.Invoke(() =>
    {
      listView.UpdateLayout();
      listView.FitLastColumnWidthEnable((bool)e.NewValue);
      listView.UpdateLayout();
    });
  }

  private static void FitLastColumnWidthEnable(this ListView listView, bool value)
  {
    if (value)
    {
      listView.Loaded += ListView_Loaded;
      listView.SizeChanged += ListView_SizeChanged;
    }
    else
    {
      listView.Loaded -= ListView_Loaded;
      listView.SizeChanged -= ListView_SizeChanged;
    }
  }

  private static void ListView_Loaded(object? sender, RoutedEventArgs e)
  {
    //Debug.WriteLine("ListView_Loaded");
    if (sender is ListView listView)
    {
      if (listView.View is GridView gridView)
        foreach (var column in gridView.Columns)
        {
          RegisterColumn(column, listView);
          if (column is INotifyPropertyChanged observableColumn)
            observableColumn.PropertyChanged += Column_PropertyChanged;
        }
      FitLastColumn(listView);
    }
  }

  /// <summary>
  /// Registering columns needed as there is no backward relationship between GridViewColumn an its parent ListView
  /// </summary>
  /// <param name="column"></param>
  /// <param name="listView"></param>
  private static void RegisterColumn(GridViewColumn column, ListView listView)
  {
    if (!ColumnsMapping.ContainsKey(column))
      ColumnsMapping.Add(column, listView);
  }

  private static Dictionary<GridViewColumn, ListView> ColumnsMapping = new Dictionary<GridViewColumn, ListView>();


  private static void Column_PropertyChanged(object? sender, PropertyChangedEventArgs args)
  {
    //Debug.WriteLine("Column_PropertyChanged");
    if (sender is GridViewColumn column)
    {
      if (ColumnsMapping.TryGetValue(column, out var listView))
      {
        if (GetFitLastColumnWidth(listView))
        {
          if (listView.View is GridView gridView)
            if (column != gridView.Columns.LastOrDefault())
            {
              if (args.PropertyName == "ActualWidth")
              {
                //Debug.WriteLine($"Column_PropertyChanged({sender}, {args.PropertyName})");
                FitLastColumn(listView);
              }
            }
        }
      }
    }
  }

  private static void ListView_SizeChanged(object sender, SizeChangedEventArgs args)
  {
    if (sender is ListView listView)
      FitLastColumn(listView);
  }

  private static void FitLastColumn(ListView listView)
  {
    if (listView.View is GridView gridView)
    {
      var workingWidth = listView.ActualWidth
      - SystemParameters.VerticalScrollBarWidth
      - 10; // Additional margin set in ListView template
      double sumWidth = 0;
      int n = gridView.Columns.Count - 1;
      for (int i = 0; i < n; i++)
        sumWidth += gridView.Columns[i].ActualWidth;
      if (sumWidth < workingWidth - 20)
        gridView.Columns[n].Width = workingWidth - sumWidth;
    }
  }

  #endregion FitLastColumnWidth

  #region Get Visuals

  private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
  {
    List<T> visualCollection = new List<T>();
    if (parent is DependencyObject dependencyObject)
      GetVisualChildCollection(dependencyObject, visualCollection);
    return visualCollection;
  }

  private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
  {
    int count = VisualTreeHelper.GetChildrenCount(parent);
    for (int i = 0; i < count; i++)
    {
      DependencyObject child = VisualTreeHelper.GetChild(parent, i);
      if (child is T tChild)
      {
        visualCollection.Add(tChild);
      }
      if (child != null)
      {
        GetVisualChildCollection(child, visualCollection);
      }
    }
  }

  #endregion // Get Visuals

  //#region ScrollIntoView
  //public static object GetScrollIntoView(DependencyObject target)
  //{
  //  return target.GetValue(ScrollIntoViewProperty);
  //}

  //public static void SetScrollIntoView(DependencyObject target, object value)
  //{
  //  target.SetValue(ScrollIntoViewProperty, value);
  //}

  //private static void OnScrollIntoViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  //{
  //  var listView = sender as ListView;
  //  if (listView == null || e.NewValue == null)
  //    return;
  //  listView.Dispatcher.Invoke(() =>
  //  {
  //    listView.UpdateLayout();
  //    listView.ScrollIntoView(e.NewValue);
  //    listView.UpdateLayout();
  //  });
  //}

  //public static readonly DependencyProperty ScrollIntoViewProperty = DependencyProperty.RegisterAttached(
  //    "ScrollIntoView",
  //    typeof(object),
  //    typeof(ListViewBehavior),
  //    new PropertyMetadata(default(object), OnScrollIntoViewChanged));

  //#endregion ScrollIntoView

}
