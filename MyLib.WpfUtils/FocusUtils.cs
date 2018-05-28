using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLib.WpfUtils
{
  public static class FocusUtils
  {
    public static bool HasFocus(this Window parentWindow, Control aControl, bool aCheckChildren)
    {
      var oFocused = System.Windows.Input.FocusManager.GetFocusedElement(parentWindow) as DependencyObject;
      if (!aCheckChildren)
        return oFocused == aControl;
      while (oFocused != null)
      {
        if (oFocused == aControl)
          return true;
        oFocused = System.Windows.Media.VisualTreeHelper.GetParent(oFocused);
      }
      return false;
    }
  }
}
