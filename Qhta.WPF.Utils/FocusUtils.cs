using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qhta.WPF.Utils
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
        if (oFocused is Visual)
          oFocused = System.Windows.Media.VisualTreeHelper.GetParent(oFocused);
        else
          return false;
      }
      return false;
    }
  }
}
