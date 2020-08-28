using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Collection of ListView behavior
  /// </summary>
  public static partial class ListViewBehavior
  {

    #region ScrollIntoView
    public static readonly DependencyProperty ScrollIntoViewProperty = DependencyProperty.RegisterAttached(
        "ScrollIntoView",
        typeof(object),
        typeof(ListViewBehavior),
        new PropertyMetadata(default(object), OnScrollIntoViewChanged));

    public static object GetScrollIntoView(DependencyObject target)
    {
      return target.GetValue(ScrollIntoViewProperty);
    }

    public static void SetScrollIntoView(DependencyObject target, object value)
    {
      target.SetValue(ScrollIntoViewProperty, value);
    }

    private static void OnScrollIntoViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var listView = sender as ListView;
      if (listView == null || e.NewValue == null)
        return;
      listView.Dispatcher.Invoke(() =>
      {
        listView.UpdateLayout();
        listView.ScrollIntoView(e.NewValue);
        listView.UpdateLayout();
      });
    }

    #endregion ScrollIntoView

    #region FitLastColumnWidth
    public static readonly DependencyProperty FitLastColumnWidthProperty = DependencyProperty.RegisterAttached(
        "FitLastColumnWidth",
        typeof(bool),
        typeof(ListViewBehavior),
        new PropertyMetadata(default(bool), OnFitLastColumnWidthChanged));

    public static bool GetFitLastColumnWidth(DependencyObject target)
    {
      return (bool)target.GetValue(FitLastColumnWidthProperty);
    }

    public static void SetFitLastColumnWidth(DependencyObject target, bool value)
    {
      target.SetValue(FitLastColumnWidthProperty, value);
    }

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

    public static void FitLastColumnWidthEnable(this ListView listView, bool value)
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

    private static void ListView_Loaded(object sender, RoutedEventArgs e)
    {
      //Debug.WriteLine("ListView_Loaded");
      if (sender is ListView listView)
      {
        GridView gridView = listView.View as GridView;
        foreach (var column in gridView.Columns)
        {
          RegisterColumn(column, listView);
          (column as INotifyPropertyChanged).PropertyChanged += Column_PropertyChanged;
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


    private static void Column_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      //Debug.WriteLine("Column_PropertyChanged");
      if (sender is GridViewColumn column)
      {
        if (ColumnsMapping.TryGetValue(column, out var listView))
        {
          if (GetFitLastColumnWidth(listView))
          {
            GridView gridView = listView.View as GridView;
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
      GetVisualChildCollection(parent as DependencyObject, visualCollection);
      return visualCollection;
    }

    private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
    {
      int count = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < count; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(parent, i);
        if (child is T)
        {
          visualCollection.Add(child as T);
        }
        if (child != null)
        {
          GetVisualChildCollection(child, visualCollection);
        }
      }
    }

    #endregion // Get Visuals

  }
}
