namespace Qhta.Conversion;

/// <summary>
/// Converts a Unicode string to its serializable equivalent string (and vice versa). 
/// When it meets invisible character, it can use EscapeSequences (like "\t" "\n" "\r"), Html entities or Hex entities. 
/// EscapeSequences and HexEntities are predefined, but may be redefined by the programmer.
/// It also supports Patterns and Enumerations on ConvertFrom (they can be case-insensitive).
/// </summary>
public class StringTypeConverter : BaseTypeConverter, ILengthRestrictions, ITextRestrictions, IWhitespaceRestrictions
{
  /// <summary>
  /// Basic converter from System.ComponentModel.
  /// </summary>
  public StringConverter Base = new StringConverter();

  /// <summary>
  /// Default constructor specifies that ExpectedType is string.
  /// </summary>
  public StringTypeConverter()
  {
    ExpectedType = typeof(string);
  }

  /// <summary>
  /// Specifies what to do with whitespaces.
  /// </summary>
  protected WhitespaceBehavior _Whitespaces;

  /// <summary>
  /// Specifies whether escape sequences like "\t" "\n" "\r" should be used for control characters.
  /// </summary>
  public bool UseEscapeSequences { get; set; }

  /// <summary>
  /// Specifies whether Html sequences should be used for some characters.
  /// </summary>
  public bool UseHtmlEntities { get; set; }

  /// <summary>
  /// Specifies whether Hex entities like "\xFF" should be used for control characters.
  /// </summary>
  public bool UseHexEntities { get; set; }

  /// <summary>
  /// Predefined escape sequences.
  /// </summary>
  public BiDiDictionary<char, string> EscapeSequences { get; set; } = new()
  {
    { '\0', "\\0" },
    { '\u0007', "\\a" },
    { '\u0008', "\\b" },
    { '\u0009', "\\t" },
    { '\u000A', "\\n" },
    { '\u000B', "\\v" },
    { '\u000C', "\\f" },
    { '\u000D', "\\r" },
    { '\u0022', "\\\"" },
    { '\u0027', "\\'" },
    { '\u005C', "\\\\" }
  };

  /// <summary>
  /// Predefined Html entities
  /// </summary>
  public BiDiDictionary<char, string> HtmlEntities { get; set; } = new()
  {
    { '\xA0', "&nbsp;" },
    { '<', "&lt;" },
    { '>', "&gt;" },
    { '&', "&amp;" },
    { '"', "&quot;" },
    { '\'', "&apos;" }
  };

  /// <summary>
  /// Min length of the string
  /// </summary>
  public int? MinLength { get; set; }
  /// <summary>
  /// Max length of the string
  /// </summary>
  public int? MaxLength { get; set; }

  /// <summary>
  /// Specifies regular expression patterns to be used on ConvertFrom.
  /// </summary>
  public string[]? Patterns { get; set; }

  /// <summary>
  /// Specifies acceptable string enumerations to be used on ConvertFrom.
  /// </summary>
  public string[]? Enumerations { get; set; }

  /// <summary>
  /// Specifies whether backward conversion is insensitive 
  /// when checking Patterns or Enumerations.
  /// </summary>
  public bool CaseInsensitive { get; set; }

  /// <summary>
  /// Specifies an XsdType with filling Patterns and MinLength.
  /// </summary>
  public override XsdSimpleType? XsdType
  {
    get => _XsdType;
    set
    {
      switch (value)
      {
        case XsdSimpleType.String:
          Whitespaces = WhitespaceBehavior.Preserve;
          break;
        case XsdSimpleType.NormalizedString:
          Whitespaces = WhitespaceBehavior.Replace;
          break;
        case XsdSimpleType.Token:
          Whitespaces = WhitespaceBehavior.Collapse;
          MinLength = 1;
          break;
        case XsdSimpleType.NmToken:
          Whitespaces = WhitespaceBehavior.Collapse;
          MinLength = 1;
          Patterns = new[] { @"(\p{L}|\p{N}|\p{M}|[-.:])+" };
          break;
        case XsdSimpleType.Name:
          Whitespaces = WhitespaceBehavior.Collapse;
          MinLength = 1;
          Patterns = new[] { @"(\p{L}|\p{M}|[:_])(\p{L}|\p{N}|\p{M}|[-.:_])*" };
          break;
        case XsdSimpleType.NcName:
        case XsdSimpleType.Id:
        case XsdSimpleType.IdRef:
        case XsdSimpleType.Entity:
          Whitespaces = WhitespaceBehavior.Collapse;
          MinLength = 1;
          Patterns = new[] { @"(\p{L}|\p{M}|[_])(\p{L}|\p{N}|\p{M}|[-._])*" };
          break;
        case XsdSimpleType.Language:
          Whitespaces = WhitespaceBehavior.Collapse;
          MinLength = 1;
          Patterns = new[] { @"([a-zA-Z]{2}|[a-zA-Z]{3}|[iI]-[a-zA-Z]+|[xX]-[a-zA-Z]{1,8})(-[a-zA-Z]{1,8})*" };
          break;
        case XsdSimpleType.Notation:
          Whitespaces = WhitespaceBehavior.Collapse;
          WhitespacesFixed = true;
          break;
        default:
          return;
      }
      _XsdType = value;
    }
  }
  /// <summary>
  /// Protected XsdType.
  /// </summary>
  protected XsdSimpleType? _XsdType;


  /// <summary>
  /// Specifies whether a Whitespaces property can't be changed.
  /// </summary>
  public bool WhitespacesFixed { get; set; }

  /// <summary>
  /// Changes Whitespaces only when WhitespacesFixed is false.
  /// </summary>
  public WhitespaceBehavior Whitespaces
  {
    get => _Whitespaces;
    set
    {
      if (!WhitespacesFixed)
        _Whitespaces = value;
    }
  }

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
    if (value is char ch)
      value = ch.ToString();
    if (value is string str)
    {
      if (Whitespaces != 0)
        str = ValidateWhitespaces(str, Whitespaces);
      str = ValidateStrLength(str, MinLength, MaxLength);
      if (Patterns != null)
        ValidatePatterns(str, Patterns);
      if (Enumerations != null)
        ValidateEnumerations(str, Enumerations);

      if (UseEscapeSequences)
        return EncodeEscapeSequences(str);
      if (UseHtmlEntities)
        return EncodeHtmlEntities(str);
      return str;
    }
    return Base.ConvertTo(context, culture, value, destinationType);
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
      if (UseEscapeSequences)
        str = DecodeEscapeSequences(str);
      else if (UseHtmlEntities)
        str = DecodeHtmlEntities(str);
      if (Whitespaces != 0)
        str = ValidateWhitespaces(str, Whitespaces);
      str = ValidateStrLength(str, MinLength, MaxLength);
      if (Patterns != null)
        ValidatePatterns(str, Patterns);
      if (Enumerations != null)
        ValidateEnumerations(str, Enumerations);

      if (ExpectedType == typeof(char))
        return str.FirstOrDefault();
      if (ExpectedType == null || ExpectedType == typeof(string))
        return str;
      return Convert.ChangeType(str, ExpectedType ?? typeof(string));
    }
    return Convert.ChangeType(value, ExpectedType ?? typeof(string));
  }

  /// <summary>
  /// Encodes escape sequences in a string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public string EncodeEscapeSequences(string str)
  {
    var sb = new StringBuilder();
    var start = 0;
    for (var i = 0; i < str.Length; i++)
    {
      var ch = str[i];
      string? entity = null;
      if (char.IsControl(ch) || EscapeSequences.TryGetValue2(ch, out entity))
      {
        if (i > start)
          sb.Append(str.Substring(start, i - start));
        start = i + 1;
        if (!String.IsNullOrEmpty(entity))
        {
          sb.Append(entity);
        }
        else if (ch <= '\x7F')
        {
          if (UseHexEntities)
            sb.Append($"\\x{(byte)ch:X2}");
          else
            sb.Append($"\\u00{(byte)ch:X2}");
        }
        else
        {
          if (UseHexEntities)
            sb.Append($"\\x{(UInt16)ch:X4}");
          else
            sb.Append($"\\u{(UInt16)ch:X4}");
        }
      }
    }
    if (start < str.Length)
      sb.Append(str.Substring(start, str.Length - start));
    var result = sb.ToString();
    return result;
  }

  /// <summary>
  /// Decodes escape sequences in a string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public string DecodeEscapeSequences(string str)
  {
    var sb = new StringBuilder();
    var start = 0;
    for (var i = 0; i < str.Length; i++)
      if (i < str.Length - 2 && str[i] == '\\')
      {
        char? charValue = null;
        var k = 0;
        if (str[i + 1] is 'x' or 'u')
        {
          for (var j = i + 2; j < str.Length && j <= i + 6; j++)
          {
            var ch = str[j];
            if (ch is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f')
              k = j;
            else
              break;
          }
          if (k > i)
            charValue = Convert.ToChar(Convert.ToInt32(str.Substring(i + 2, k - i - 1), 16));
        }
        else if (EscapeSequences.TryGetValue1(str.Substring(i, 2), out var charVal))
        {
          charValue = charVal;
          k = i + 1;
        }
        if (charValue != null)
        {
          if (start < i)
            sb.Append(str.Substring(start, i - start));
          sb.Append(charValue);
          i = k;
          start = i + 1;
        }
      }
    if (start < str.Length)
      sb.Append(str.Substring(start, str.Length - start));
    var result = sb.ToString();
    return result;
  }

  /// <summary>
  /// Encodes HtmlEntities in a string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public string EncodeHtmlEntities(string str)
  {
    var sb = new StringBuilder();
    var start = 0;
    for (var i = 0; i < str.Length; i++)
    {
      var ch = str[i];
      string? entity = null;
      if (char.IsControl(ch) || HtmlEntities.TryGetValue2(ch, out entity))
      {
        if (i > start)
          sb.Append(str.Substring(start, i - start));
        start = i + 1;
        if (!String.IsNullOrEmpty(entity))
        {
          sb.Append(entity);
        }
        else if (ch <= '\x7F')
        {
          if (UseHexEntities)
            sb.Append($"&#x{(byte)ch:X2};");
          else
            sb.Append($"&#{(byte)ch};");
        }
        else
        {
          if (UseHexEntities)
            sb.Append($"&#x{(UInt16)ch:X4};");
          else
            sb.Append($"&#{(UInt16)ch};");
        }
      }
    }
    if (start < str.Length)
      sb.Append(str.Substring(start, str.Length - start));
    var result = sb.ToString();
    return result;
  }

  /// <summary>
  /// Decodes HtmlEntities in a string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public string DecodeHtmlEntities(string str)
  {
    var sb = new StringBuilder();
    var start = 0;
    for (var i = 0; i < str.Length; i++)
      if (i < str.Length - 1 && str[i] == '&')
      {
        var k = str.IndexOf(';', i + 2, Math.Min(str.Length - i - 2, 10));
        if (k > i)
        {
          char? charValue = null;
          if (i < str.Length - 2 && str[i + 1] == '#')
          {
            if (str[i + 2] == 'x')
              charValue = Convert.ToChar(Convert.ToInt32(str.Substring(i + 3, k - i - 3), 16));
            else if (str[i + 2] is >= '0' && str[i + 2] <= '9')
              charValue = Convert.ToChar(Convert.ToInt32(str.Substring(i + 2, k - i - 2), 10));
          }
          else if (HtmlEntities.TryGetValue1(str.Substring(i, k - i + 1), out var charVal))
          {
            charValue = charVal;
          }
          if (charValue != null)
          {
            if (start < i)
              sb.Append(str.Substring(start, i - start));
            sb.Append(charValue);
            i = k;
            start = i + 1;
          }
        }
      }
    if (start < str.Length)
      sb.Append(str.Substring(start, str.Length - start));
    var result = sb.ToString();
    return result;
  }

  /// <summary>
  /// Validates string length. Adds spaces or trims a string.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="minLength"></param>
  /// <param name="maxLength"></param>
  /// <returns></returns>
  public static string ValidateStrLength(string str, int? minLength, int? maxLength)
  {
    if (maxLength != null && str.Length > maxLength)
      str = str.Substring(0, (int)maxLength);
    if (minLength != null && str.Length < minLength)
      str += new string(' ', (int)minLength - str.Length);
    return str;
  }

  /// <summary>
  /// Validates string against WhitespaceBehavior. Returns converted string.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="behavior"></param>
  /// <returns></returns>
  public static string ValidateWhitespaces(string str, WhitespaceBehavior behavior)
  {
    var chars = str.ToArray();
    var replaced = false;
    for (var i = 0; i < str.Length; i++)
    {
      var ch = chars[i];
      if (char.IsWhiteSpace(ch) && ch != ' ')
      {
        chars[i] = ' ';
        replaced = true;
      }
    }
    if (replaced)
      str = new string(chars);
    if (behavior == WhitespaceBehavior.Collapse)
    {
      str = str.Trim();
      for (var i = str.Length - 1; i > 0; i--)
        if (str[i] == ' ' && str[i - 1] == ' ')
          str = str.Remove(i, 1);
    }
    return str;
  }

  /// <summary>
  /// Validates patterns. 
  /// If the string does not match to any pattern, then InvalidOperationException is thrown.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="patterns"></param>
  /// <exception cref="InvalidOperationException"></exception>
  public void ValidatePatterns(string str, string[] patterns)
  {
    var ok = patterns.Length == 0;
    foreach (var pattern in patterns)
      if (ValidatePattern(str, pattern))
      {
        ok = true;
        break;
      }
    if (!ok)
    {
      var msg = $"Invalid string \"{str}\" in StringTypeConverter";
      if (XsdType != null)
        msg += $" with XsdType={XsdType}";
      throw new InvalidOperationException(msg);
    }
  }

  /// <summary>
  /// Validates a single pattern.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="pattern"></param>
  /// <returns></returns>
  public bool ValidatePattern(string str, string pattern)
  {
    pattern = @"\A" + pattern + @"\Z";
    var regex = new Regex(pattern);
    return regex.Match(str).Success;
  }

  /// <summary>
  /// Validates enumeration 
  /// If the string does not match to any enumeration, then InvalidOperationException is thrown.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="enumerations"></param>
  /// <exception cref="InvalidOperationException"></exception>
  public void ValidateEnumerations(string str, string[] enumerations)
  {
    var ok = enumerations.Length == 0;
    for (var i = 0; i < enumerations.Length; i++)
      if (string.Equals(str, enumerations[i],
            CaseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
      {
        ok = true;
        break;
      }
    if (!ok)
    {
      var msg = $"No enumeration encompassed for \"{str}\" in StringTypeConverter";
      if (XsdType != null)
        msg += $" with XsdType={XsdType}";
      throw new InvalidOperationException(msg);
    }
  }
}