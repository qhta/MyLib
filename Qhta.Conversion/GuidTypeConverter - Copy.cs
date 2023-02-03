using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class DbNullTypeConverter : TypeConverter, ITypeConverter
{
  /// <summary>
  ///   Do not change
  /// </summary>
  public Type? ExpectedType { get; set; } = typeof(DBNull);

  /// <summary>
  ///   Do not change
  /// </summary>
  public XsdSimpleType? XsdType { get; set; } = XsdSimpleType.String;

  /// <summary>
  ///   Not used
  /// </summary>
  public string? Format { get; set; }

  /// <summary>
  ///   Not used
  /// </summary>
  public CultureInfo? Culture { get; set; }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    if (value is DBNull)
      return null!;
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    return DBNull.Value;
  }
}