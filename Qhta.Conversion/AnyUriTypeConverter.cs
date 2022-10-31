using System.Buffers.Text;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Qhta.Conversion;


public class AnyUriTypeConverter : UriTypeConverter, ITypeConverter
{
  /// <summary>
  /// Do not change
  /// </summary>
  public Type? ExpectedType { get; set; } = typeof(Uri);
  /// <summary>
  /// Do not change
  /// </summary>
  public XsdSimpleType? XsdType { get; set; } = XsdSimpleType.AnyUri;

  /// <summary>
  /// Not used
  /// </summary>
  public string? Format { get; set; }

  /// <summary>
  /// Not used
  /// </summary>
  public CultureInfo? Culture { get; set; }

  public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null!;
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value == null)
      return null;
    return base.ConvertFrom(context, culture, value);
  }
}