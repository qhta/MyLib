namespace Qhta.WPF.Utils;

/// <summary>
/// Utility class that gets a parent window of the framework element.
/// </summary>
public static class FrameworkElementUtils
{

  /// <summary>
  /// Obsolete method to find a parent window od the framework element.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  [Obsolete]
  public static Window? FindParentWindow(this FrameworkElement element)
  {
    object? parent = element.Parent;
    while (parent != null && !(parent is Window))
      parent = (parent as FrameworkElement)?.Parent;
    return parent as Window;
  }

  /// <summary>
  /// Method to get a parent window od the framework element.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static Window? GetParentWindow(this FrameworkElement element)
  {
    object? parent = element.Parent;
    while (parent != null && !(parent is Window))
      parent = (parent as FrameworkElement)?.Parent;
    return parent as Window;
  }
}
