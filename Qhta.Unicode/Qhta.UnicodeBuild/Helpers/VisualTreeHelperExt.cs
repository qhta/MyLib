using System.Windows.Media;
using System.Windows;

namespace Qhta.UnicodeBuild.Helpers;

public static class VisualTreeHelperExt
{
  public static T? FindParent<T>(this DependencyObject child) where T : DependencyObject
  {
    if (child is FrameworkElement fe)
    {
      if (fe.Parent is T TParent) return TParent;
      if (fe.Parent != null) return FindParent<T>(fe.Parent);
      if (fe.TemplatedParent is T TTemplatedParent) return TTemplatedParent;
      if (fe.TemplatedParent != null) return FindParent<T>(fe.TemplatedParent);
    }
    return null;
    //DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
    //if (parentObject == null) return null;

    //if (parentObject is T parent)
    //  return parent;
    //return FindParent<T>(parentObject);
  }
}