using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Qhta.Collections;

namespace Qhta.Conversion;

public class StringTypeConverter : BaseTypeConverter, ILengthRestrictions, ITextRestrictions, IWhitespaceRestrictions
{
  public StringConverter Base = new StringConverter();

  public StringTypeConverter()
  {
    ExpectedType = typeof(string);
  }

  protected WhitespaceBehavior _Whitespaces;

  protected XsdSimpleType? _XsdType;

  public bool UseEscapeSequences { get; set; }
  public bool UseHtmlEntities { get; set; }
  public bool HexEntities { get; set; }

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

  public BiDiDictionary<char, string> HtmlEntities { get; set; } = new()
  {
    { '\xA0', "&nbsp;" },
    { '<', "&lt;" },
    { '>', "&gt;" },
    { '&', "&amp;" },
    { '"', "&quot;" },
    { '\'', "&apos;" }
  };

  public int? MinLength { get; set; }
  public int? MaxLength { get; set; }

  public string[]? Patterns { get; set; }
  public string[]? Enumerations { get; set; }
  public bool CaseInsensitive { get; set; }

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

  public WhitespaceBehavior Whitespaces
  {
    get => _Whitespaces;
    set
    {
      if (!WhitespacesFixed)
        _Whitespaces = value;
    }
  }

  public bool WhitespacesFixed { get; set; }

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

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
          if (HexEntities)
            sb.Append($"\\x{(byte)ch:X2}");
          else
            sb.Append($"\\u00{(byte)ch:X2}");
        }
        else
        {
          if (HexEntities)
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
          if (HexEntities)
            sb.Append($"&#x{(byte)ch:X2};");
          else
            sb.Append($"&#{(byte)ch};");
        }
        else
        {
          if (HexEntities)
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

  public static string ValidateStrLength(string str, int? minLength, int? maxLength)
  {
    if (maxLength != null && str.Length > maxLength)
      str = str.Substring(0, (int)maxLength);
    if (minLength != null && str.Length < minLength)
      str += new string(' ', (int)minLength - str.Length);
    return str;
  }

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

  public bool ValidatePattern(string str, string pattern)
  {
    pattern = @"\A" + pattern + @"\Z";
    var regex = new Regex(pattern);
    return regex.Match(str).Success;
  }

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