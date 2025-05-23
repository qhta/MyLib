using System.Windows.Media;
using System.Windows;

namespace Qhta.UnicodeBuild.Helpers;

public static class VisualTreeHelperExt
{
  public static T? FindParent<T>(this DependencyObject child) where T : DependencyObject
  {
    DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
    if (parentObject == null) return null;

    if (parentObject is T parent)
      return parent;
    return FindParent<T>(parentObject);
  }
}