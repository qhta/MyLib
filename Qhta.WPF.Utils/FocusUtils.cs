namespace Qhta.WPF.Utils;

/// <summary>
/// Utility class that checks whether the window has a control with focus.
/// </summary>
public static class FocusUtils
{
  /// <summary>
  /// Checks whether the window has a control with focus.
  /// </summary>
  /// <param name="parentWindow"></param>
  /// <param name="aControl"></param>
  /// <param name="aCheckChildren"></param>
  /// <returns></returns>
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
