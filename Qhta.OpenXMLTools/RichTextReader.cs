using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class for reading OpenXml documents with rich text.
/// </summary>
public class RichTextReader: OpenXmlTextReader
{

  /// <summary>
  /// Read a string
  /// </summary>
  public override string ReadString()
  {
    var sb = new StringBuilder();
    while (!EOF())
    {
      var str = ReadCharStr();
      sb.Append(str);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Read a single char
  /// </summary>
  public override char ReadChar()
  {
    var result = ReadCharStr();
    if (result.Length != 1)
      throw new InvalidOperationException("Expected a single character.");
    return result[0];
  }

  /// <summary>
  /// Read a char as string.
  /// When reading function control word (as \sup, \sub, etc.), the function reads the parameter in braces and the result can be a string with more than one character.
  /// </summary>
  public string ReadCharStr()
  {
    var str = (char)base.ReadChar();
    if (str==' ')
      Debug.Assert(true);
    if (str== '\\')
    {
      if (Peek() == '\\')
      {
        base.ReadChar();
        return "\\";
      }
      return ReadCharWithName();
    }
    return new string(str,1);
  }

  private string ReadCharWithName()
  {
    var sb = new StringBuilder();
    if (!EOF())
    {
      if (TryReadUnicodeHex(out char ch))
        return new string(ch, 1);
      ch = base.Peek();
      if (!char.IsLetter(ch))
        return new string('\\',1);
      while (!EOF())
      {
        ch = Peek();
        if (ch == ' ')
        {
          base.ReadChar();
          break;
        }
        if (ch == '\\' || ch == '{')
          break;
        base.ReadChar();
        sb.Append(ch);

      }
      var charName = sb.ToString();
      if (charName.StartsWith("sup") || charName.StartsWith("sub"))
      {
        Debug.Assert(true);
        var param = "";
        if (ch == '{')
        {
          sb.Clear();
          base.ReadChar();
          while (!EOF())
          {
            ch = base.ReadChar();
            if (ch == '}')
              break;
            sb.Append(ch);
          }
          param = sb.ToString();
        }
        sb.Clear();
        foreach (var ch1 in param)
        {
          var seq = charName + "{" + ch1 + "}";
          if (CharNames.TryFindFunction(seq, out var cp))
            sb.Append((char)cp);
          else
            throw new InvalidOperationException($"Invalid \\{charName} parameter \"{param}\"");
        }
        return sb.ToString();
      }
      else
      {
        if (CharNames.TryFindName(charName, out var cp))
          return new string((char)cp, 1);
        throw new InvalidOperationException("Invalid character name.");
      }
    }
    throw new InvalidOperationException("No more characters available.");
  }

  private bool TryReadUnicodeHex(out char ch)
  {
    ch = (char)0;
    if (Peek() == '\'')
    {
      base.ReadChar();
      if (!IsHexDigit(Peek()))
      {
        Seek(-1, SeekOrigin.Current);
        return false;
      }
      int value = 0;
      while (IsHexDigit((char)Peek()))
      {
        ch = base.ReadChar();
        value = value * 16 + HexDigitVal(ch);
      }
      if (value > 0xFFFF)
        throw new InvalidOperationException("Invalid Unicode character value.");
      ch = (char)value;
      return true;
    }
    return false;
  }

  private bool IsHexDigit(char ch)
  {
    return (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'F');
  }

  private int HexDigitVal(char ch)
  {
    return (ch >= 'A' ? ch - 'A' + 10 : ch - '0');
  }

}
