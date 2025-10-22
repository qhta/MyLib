namespace Qhta.Conversion;

/// <summary>
/// Converts a Boolean value to/from string using true/false, 1/0, on/off pairs.
/// </summary>
public class BooleanTypeConverter : BaseTypeConverter, ITextRestrictions
{
  /// <summary>
  /// Sets Expected type to Boolean.
  /// </summary>
  public BooleanTypeConverter()
  {
    ExpectedType = typeof(Boolean);
  }

  /// <summary>
  /// Definition of strings pairs that are used to represent true and false boolean values.
  /// The first string of the pair represents true value and the second string represents false value.
  /// This property is public and can be changed for the specific converter instance.
  /// </summary>
  public (string, string)[] BooleanStrings { get; set; }
    = { ("True", "False"), ("1", "0"), ("on", "off") };

  /// <summary>
  ///   ITextRestrictions patterns unused in this converter
  /// </summary>
  public string[]? Patterns { get; set; }

  /// <summary>
  ///   BooleanStrings represented as a single-dimension string array.
  ///   This array must have even number of strings.
  ///   Each event string represents true value and odd string represents false value.
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

  /// <summary>
  /// Specifies whether backward conversion is case insensitive.
  /// </summary>
  public bool CaseInsensitive { get; set; } = true;


  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <inheritdoc/>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is null)
      return null;
    if (value is bool bv)
    {
      var mode = 0;
      if (SimpleType == Xml.SimpleType.Int || SimpleType == Xml.SimpleType.Integer)
        mode = 1;
      else if (SimpleType == Xml.SimpleType.String || SimpleType == Xml.SimpleType.NormalizedString)
        mode = 2;

      if (destinationType == typeof(string))
      {
        var result = bv ? BooleanStrings[mode].Item1 : BooleanStrings[mode].Item2;
        return result;
      }
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <inheritdoc/>
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