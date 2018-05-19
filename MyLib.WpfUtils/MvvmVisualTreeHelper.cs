using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyLib.WpfUtils
{
  public static class MvvmVisualTreeHelper
  {
    public static VisualType FindVisualChildForDataContext<VisualType>(DependencyObject parent, object dataObject) where VisualType: Visual
    {
      int n = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i<n; i++)
      {
        var child = VisualTreeHelper.GetChild(parent, i);
        if (child is VisualType result)
        {
          var dataContext = child.GetValue(Control.DataContextProperty);
          //Debug.WriteLine($"Child type = {child.GetType().Name}");
          var value = dataContext.GetType().GetProperty("Number").GetValue(dataContext);
          //Debug.WriteLine($"DataContext = {dataContext} Number = {value}");
          if (dataContext==dataObject)
          {
            return result;
          }
        }
        var recursiveResult = FindVisualChildForDataContext<VisualType>(child, dataObject);
        if (recursiveResult!=null)
          return recursiveResult;
      }
      return null;
    }
  }
}
