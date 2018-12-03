namespace Qhta.Drawing
{
  /// <summary>
  /// Enumeration type describing how the Hue value is treated by <see cref="ColorIterator"/> 
  /// when there is no difference between hue of start and end color.
  /// </summary>
  public enum HueChange
  {
    /// <summary>
    /// Hue increment evaluated as a difference between end hue and start hue
    /// </summary>
    Undefined,
    /// <summary>
    /// No Hue increment
    /// </summary>
    None,
    /// <summary>
    /// Positive Hue increment (from red to violet) when the hue difference equals 0
    /// </summary>
    Positive,
    /// <summary>
    /// Negative Hue increment (from violet to red) when the hue difference equals 0
    /// </summary>
    Negative,
    /// <summary>
    /// Hue increment is evaluated to create the shortest path between start hue and end hue
    /// </summary>
    Shortest
  }
}
