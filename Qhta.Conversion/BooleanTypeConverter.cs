using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class BooleanTypeConverter : BaseTypeConverter, ITextRestrictions
{
  public BooleanTypeConverter()
  {
    ExpectedType = typeof(Boolean);
  }

  public (string, string)[] BooleanStrings { get; set; }
    = { ("True", "False"), ("1", "0"), ("on", "off") };

  /// <summary>
  ///   Unused for this converter
  /// </summary>
  public string[]? Patterns { get; set; }

  /// <summary>
  ///   Set BooleanStrings
  /// </summary>
  public string[]? Enumerations
  {
    get
    {
      var strs = new List<string>();
      foreach (var ps in BooleanStrings)
      {
        strs.Add(ps.Item1);
        strs.Add(ps.Item2);
      }
      return strs.ToArray();
    }
    set
    {
      if (value != null)
        for (var i = 0; i < value.Length / 2; i += 2)
          BooleanStrings[i] = (value[i * 2], value[i * 2 + 1]);
    }
  }

  public bool CaseInsensitive { get; set; } = true;

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is null)
      return null;
    if (value is bool bv)
    {
      var mode = 0;
      if (XsdType == XsdSimpleType.Int || XsdType == XsdSimpleType.Integer)
        mode = 1;
      else if (XsdType == XsdSimpleType.String || XsdType == XsdSimpleType.NormalizedString)
        mode = 2;

      if (destinationType == typeof(string))
      {
        var result = bv ? BooleanStrings[mode].Item1 : BooleanStrings[mode].Item2;
        return result;
      }
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value is null)
      return null;
    if (value is string str)
    {
      if (str == String.Empty)
        return null;
      str = str.ToLowerInvariant();
      StringComparison comparison;
      CultureInfo? cultureSave = null;
      if (culture == null)
      {
        comparison = CaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
      }
      else if (culture == CultureInfo.InvariantCulture)
      {
        comparison = CaseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
      }
      else
      {
        comparison = CaseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
        if (culture != CultureInfo.CurrentCulture)
        {
          cultureSave = CultureInfo.CurrentCulture;
          CultureInfo.CurrentCulture = culture;
        }
      }
      bool? ok = null;
      foreach (var bs in BooleanStrings)
      {
        if (str.Equals(bs.Item1, comparison))
        {
          ok = true;
          break;
        }
        if (str.Equals(bs.Item2, comparison))
        {
          ok = false;
          break;
        }
      }
      if (cultureSave != null)
        CultureInfo.CurrentCulture = cultureSave;
      if (ok != null)
        return ok;
    }
    return base.ConvertFrom(context, culture, value);
  }
}