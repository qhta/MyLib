using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Collection of ListView behavior
  /// </summary>
  public static class ListViewBehavior
  {


    #region ScrollIntoView
    public static readonly DependencyProperty ScrollIntoViewProperty = DependencyProperty.RegisterAttached(
        "ScrollIntoView",
        typeof(object),
        typeof(ListViewBehavior),
        new PropertyMetadata(default(object), OnScrollIntoViewChanged));

    public static object GetScrollIntoView(DependencyObject target)
    {
      return (object)target.GetValue(ScrollIntoViewProperty);
    }

    public static void SetScrollIntoView(DependencyObject target, object value)
    {
      target.SetValue(ScrollIntoViewProperty, value);
    }

    static void OnScrollIntoViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var listView = sender as ListView;
      if (listView == null || e.NewValue == null)
        return;
      listView.Dispatcher.Invoke((Action)(() =>
      {
        listView.UpdateLayout();
        listView.ScrollIntoView(e.NewValue);
        listView.UpdateLayout();
      }));
    }

    #endregion ScrollIntoView


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
