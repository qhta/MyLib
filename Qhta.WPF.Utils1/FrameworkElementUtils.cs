using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Qhta.WPF.Utils
{
  public static class FrameworkElementUtils
  {

    [Obsolete]
    public static Window FindParentWindow(this FrameworkElement element)
    {
      object parent = element.Parent;
      while (parent != null && !(parent is Window))
        parent = (parent as FrameworkElement).Parent;
      return parent as Window;
    }

    public static Window GetParentWindow(this FrameworkElement element)
    {
      object parent = element.Parent;
      while (parent != null && !(parent is Window))
        parent = (parent as FrameworkElement).Parent;
      return parent as Window;
    }
  }
}
