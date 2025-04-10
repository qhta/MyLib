﻿using System.Text;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Helper class with methods used in QXml serializer
/// </summary>
public static class QXmlSerializationHelper
{
  /// <summary>
  /// URL to XML schema instance
  /// </summary>
  public const string xsiNamespace = @"http://www.w3.org/2001/XMLSchema-instance";
  /// <summary>
  /// URL to XML schema
  /// </summary>
  public const string xsdNamespace = @"http://www.w3.org/2001/XMLSchema";

  /// <summary>
  /// Creates a tag from a Type. Tag is a full name with two exceptions:
  /// <list type="bullet">
  /// <item>
  /// When a namespace is "System", only type name is returned.
  /// </item>
  /// <item>
  /// When a full name ends with [], then it is changed to "s" suffix.
  /// </item>
  /// </list>
  /// </summary>
  /// <param name="aType">Type to get name</param>
  /// <returns>Type tag string</returns>
  public static string GetTypeTag(this Type aType)
  {
    var result = aType.FullName ?? "";
    if (result.StartsWith("System."))
      result = aType.Name;
    if (result.EndsWith("[]")) result = result.Substring(0, result.Length - 2) + "s";
    var k = result.IndexOf('`');
    if (k >= 0)
    {
      result = result.Substring(0,k);
      if (aType.IsConstructedGenericType)
      {
        var args = aType.GetGenericArguments();
        foreach (var arg in args )
          result += "_"+arg.GetTypeTag();
      }
    }
    return result;
  }

  /// <summary>
  /// Changes string case according to the specified mode
  /// </summary>
  /// <param name="str">String to change case.</param>
  /// <param name="nameCase">Mode as enumerated by <see cref="SerializationCase"/> type.</param>
  /// <returns>String with changed case</returns>
  public static string ChangeCase(this string str, SerializationCase nameCase)
  {
    switch (nameCase)
    {
      case SerializationCase.LowercaseFirstLetter:
        return FirstLetterToLower(str);
      case SerializationCase.UppercaseFirstLetter:
        return FirstLetterToUpper(str);
    }
    return str;
  }

  /// <summary>
  /// Changes first letter of the text to lowercase.
  /// </summary>
  public static string FirstLetterToLower(this string text)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    var ss = text.ToCharArray();
    for (var i = 0; i < ss.Length; i++)
      if (Char.IsLetter(ss[0]))
        ss[i] = char.ToUpper(ss[i]);
    return new string(ss);
  }

  /// <summary>
  /// Changes first letter of the text to uppercase.
  /// </summary>
  public static string FirstLetterToUpper(this string text)
  {
    if (string.IsNullOrEmpty(text))
      return text;
    var ss = text.ToCharArray();
    for (var i = 0; i < ss.Length; i++)
      if (Char.IsLetter(ss[0]))
        ss[i] = char.ToLower(ss[i]);
    return new string(ss);
  }

  /// <summary>
  /// Checkes if the first letter of the text is lowercase.
  /// </summary>
  public static bool IsFirstLetterLower(this string text)
  {
    if (string.IsNullOrEmpty(text))
      return false;
    foreach (var ch in text)
      if (char.IsLetter(ch) && !Char.IsUpper(ch))
        return false;
    return true;
  }

  /// <summary>
  /// Checkes if the first letter of the text is uppercase.
  /// </summary>
  public static bool IsFirstLetterUpper(this string text)
  {
    if (string.IsNullOrEmpty(text))
      return false;
    foreach (var ch in text)
      if (char.IsLetter(ch) && !Char.IsUpper(ch))
        return false;
    return true;
  }

  /// <summary>
  /// Encodes string value to handle invisible chars according to unicode category
  /// </summary>
  public static string EncodeStringValue(this string str)
  {
    var sb = new StringBuilder();
    foreach (var ch in str)
      if (ch == '\\')
        sb.Append("\\\\");
      else
      if (ch >= ' ' && ch < '\x7f')
      {
        sb.Append(ch);
      }
      else
      {
        var ctx = Char.GetUnicodeCategory(ch);
        switch (ctx)
        {
          case UnicodeCategory.NonSpacingMark:
          //case UnicodeCategory.SpacingCombiningMark:
          //case UnicodeCategory.EnclosingMark:
          case UnicodeCategory.SpaceSeparator:
          case UnicodeCategory.LineSeparator:
          case UnicodeCategory.ParagraphSeparator:
          case UnicodeCategory.Control:
          case UnicodeCategory.Format:
          case UnicodeCategory.Surrogate:
          case UnicodeCategory.PrivateUse:
          //case UnicodeCategory.ConnectorPunctuation:
          //case UnicodeCategory.ClosePunctuation:
          //case UnicodeCategory.OtherPunctuation:
          //case UnicodeCategory.ModifierSymbol:
          case UnicodeCategory.OtherNotAssigned:
            sb.Append(EncodeCharValue(ch));
            break;
          default:
            sb.Append(ch);
            break;
        }
      }
    return sb.ToString();
  }


  /// <summary>
  /// Encodes string value to handle control chars
  /// </summary>
  public static string EncodeCharValue(this Char ch)
  {
    switch (ch)
    {
      case '\\':
        return "\\\\";
      case '\t':
        return "\\t";
      case '\r':
        return "\\r";
      case '\n':
        return "\\n";
      case '\xA0':
        return "\\s";
      default:
        return "\\u" + ((UInt16)ch).ToString("X4");
    }
  }

  /// <summary>
  /// Decodes string value to handle encoded characters.
  /// </summary>
  public static string DecodeStringValue(this string str)
  {
    var sb = new StringBuilder();
    for (var i = 0; i < str.Length; i++)
    {
      var ch = str[i];
      if (ch == '\\' && i < str.Length - 1)
      {
        sb.Append(DecodeEscapeSeq(str, ref i));
      }
      else
      {
        sb.Append(ch);
      }
    }
    return sb.ToString();
  }

  /// <summary>
  /// Decodes a single escape char sequence.
  /// </summary>
  public static string DecodeEscapeSeq(this string str, ref int index)
  {
    var ch = str[index+1];
    switch (ch)
    {
      case '\\':
        index++;
        return "\\";
      case 't':
        index++;
        return "\t";
      case 'r':
        index++;
        return "\r";
      case 'n':
        index++;
        return "\n";
      case 's':
        index++;
        return "\xA0";
      case 'u':
        index++;
        UInt16 code = 0;
        for (var i = 1; i <= 4; i++)
        {
          var ch1 = str[index + i];
          if (ch1 >= '0' && ch1 <= '9')
            code = (UInt16)(code * 16 + (UInt16)(ch1 - '0'));
          else if (ch1 >= 'A' && ch1 <= 'F')
            code = (UInt16)(code * 16 + (UInt16)(ch1 - 'A') + 10);
          else if (ch1 >= 'a' && ch1 <= 'f')
            code = (UInt16)(code * 16 + (UInt16)(ch1 - 'a') + 10);
        }
        index += 4;
        return new string((char)code, 1);
      default:
        index++;
        return new string(ch, 1);
    }
  }
}