using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Qhta.Conversion;

public class BaseTypeConverter: TypeConverter
{
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <summary>
  ///   Type expected in ConvertFrom method
  /// </summary>
  public virtual Type? ExpectedType { get; set; }

  /// <summary>
  ///   Types known in ConvertFrom method
  /// </summary>
  public virtual Dictionary<string, Type>? KnownTypes { get; set; }

  /// <summary>
  ///   Type to use when converting to string in ConvertTo
  /// </summary>
  public virtual XsdSimpleType? XsdType { get; set; }

  /// <summary>
  ///   Format to use when converting to/from string in ConvertTo/ConvertFrom
  /// </summary>
  public virtual string? Format { get; set; }

  /// <summary>
  ///   CultureInfo to use when converting to/from string in ConvertTo/ConvertFrom
  /// </summary>
  public virtual CultureInfo? Culture { get; set; }

}