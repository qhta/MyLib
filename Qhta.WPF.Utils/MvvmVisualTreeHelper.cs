namespace Qhta.WPF.Utils;

/// <summary>
/// Utility class that contains a method working on VisualTree.
/// </summary>
public static class MvvmVisualTreeHelper
{
  /// <summary>
  /// Finds a visual child where DataContext equals dataObject.
  /// </summary>
  /// <typeparam name="VisualType"></typeparam>
  /// <param name="parent"></param>
  /// <param name="dataObject"></param>
  /// <returns></returns>
  public static VisualType? FindVisualChildForDataContext<VisualType>(DependencyObject parent, object dataObject) where VisualType : Visual
  {
    int n = VisualTreeHelper.GetChildrenCount(parent);
    for (int i = 0; i < n; i++)
    {
      var child = VisualTreeHelper.GetChild(parent, i);
      if (child is VisualType result)
      {
        var dataContext = child.GetValue(Control.DataContextProperty);
        if (dataContext != null)
        {
          //Debug.WriteLine($"Child type = {child.GetType().Name}");
          //var value = dataContext.GetType()?.GetProperty("Number")?.GetValue(dataContext);
          //Debug.WriteLine($"DataContext = {dataContext} Number = {value}");
          if (dataContext == dataObject)
          {
            return result;
          }
        }
      }
      var recursiveResult = FindVisualChildForDataContext<VisualType>(child, dataObject);
      if (recursiveResult != null)
        return recursiveResult;
    }
    return null;
  }
}
