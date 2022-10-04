using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using Qhta.Collections;

namespace Qhta.Conversion;

public class StringTypeConverter : StringConverter
{
  public bool UseEscapeSequences { get; set; }
  public bool UseHtmlEntities { get; set; }
  public bool HexEntities { get; set; }
  public BiDiDictionary<char, string> EscapeSequences { get; set; } = new BiDiDictionary<char, string>()
  {
    { '\0', "\\0"},
    { '\u0007', "\\a"},
    { '\u0008', "\\b"},
    { '\u0009', "\\t"},
    { '\u000A', "\\n"},
    { '\u000B', "\\v"},
    { '\u000C', "\\f"},
    { '\u000D', "\\r"},
    { '\u0022', "\\\""},
    { '\u0027', "\\'"},
    { '\u005C', "\\\\"},
  };

  public BiDiDictionary<char, string> HtmlEntities { get; set; } = new BiDiDictionary<char, string>()
  {
    { '\xA0', "&nbsp;" },
    { '<', "&lt;" },
    { '>', "&gt;" },
    { '&', "&amp;" },
    { '"', "&quot;" },
    { '\'', "&apos;" },
    { '¢', "&cent;" },
    { '£', "&pound;" },
    { '¥', "&yen;" },
    { '€', "&euro;" },
    { '©', "&copy;" },
    { '®', "&reg;" },
  };

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value is null)
      return null;
    if (value is string str)
    {
      if (UseEscapeSequences)
        return EncodeEscapeSequences(str);
      else
      if (UseHtmlEntities)
        return EncodeHtmlEntities(str);
      else
        return str;
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
      if (UseEscapeSequences)
        return DecodeEscapeSequences(str);
      else
      if (UseHtmlEntities)
        return DecodeHtmlEntities(str);
      else
        return str;
    }
    return base.ConvertFrom(context, culture, value);
  }

  public string EncodeEscapeSequences(string str)
  {
    var sb = new StringBuilder();
    int start = 0;
    for (int i = 0; i < str.Length; i++)
    {
      var ch = str[i];
      string? entity = null;
      if (char.IsControl(ch) || EscapeSequences.TryGetValue2(ch, out entity))
      {
        if (i > start)
          sb.Append(str.Substring(start, i - start));
        start = i + 1;
        if (!String.IsNullOrEmpty(entity))
          sb.Append(entity);
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
    int start = 0;
    for (int i = 0; i < str.Length; i++)
    {
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
        else
          if (EscapeSequences.TryGetValue1(str.Substring(i, 2), out var charVal))
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
    }
    if (start<str.Length)
      sb.Append(str.Substring(start, str.Length - start));
    var result = sb.ToString();
    return result;
  }

public string EncodeHtmlEntities(string str)
{
  var sb = new StringBuilder();
  int start = 0;
  for (int i = 0; i < str.Length; i++)
  {
    var ch = str[i];
    string? entity = null;
    if (char.IsControl(ch) || HtmlEntities.TryGetValue2(ch, out entity))
    {
      if (i > start)
        sb.Append(str.Substring(start, i - start));
      start = i + 1;
      if (!String.IsNullOrEmpty(entity))
        sb.Append(entity);
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
  int start = 0;
  for (int i = 0; i < str.Length; i++)
  {
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
        else
        if (HtmlEntities.TryGetValue1(str.Substring(i, k - i + 1), out var charVal))
          charValue = charVal;
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
  }
  if (start < str.Length)
    sb.Append(str.Substring(start, str.Length - start));
  var result = sb.ToString();
  return result;
}

}