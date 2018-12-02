namespace Qhta.WPF
{
  /// <summary>
  /// Enumeration type describing how the Hue value is treated by <see cref="ColorIterator"/> 
  /// when there is no difference between hue of start and end color.
  /// </summary>
  public enum HueChange
  {
    /// <summary>
    /// No Hue increment
    /// </summary>
    None,
    /// <summary>
    /// Positive Hue increment (from red to violet)
    /// </summary>
    Positive,
    /// <summary>
    /// Negative Hue increment (from violet to red)
    /// </summary>
    Negative
  }
}
