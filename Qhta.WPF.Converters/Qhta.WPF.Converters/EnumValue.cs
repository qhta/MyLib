namespace Qhta.WPF.Converters
{
  /// <summary>
  /// An object that replaces the enumeration value for displaying national names in selection lists.
  /// Used in <see cref="EnumValueConverter"/>.
  /// </summary>
  public class EnumValue
  {
    /// <summary>
    /// Value (converted to int).
    /// </summary>
    public object Value { get; set; } = null!;
    /// <summary>
    /// A name for conversion.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Returns a name for conversion.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return Name;
    }
  }
}
