using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class for writing OpenXml documents with rich text.
/// </summary>
public class RichTextWriter : OpenXmlTextWriter
{
  /// <summary>
  /// Write a string with rich text.
  /// </summary>
  /// <param name="value"></param>
  public override void Write(string value)
  {
    foreach (var ch in value)
    {
      Write(ch);
    }
  }

  private bool wasBackslash = false;

  /// <summary>
  /// Clear the buffer and reset the state.
  /// </summary>
  public override void Clear()
  {
    base.Clear();
    wasBackslash = false;
  }

  /// <summary>
  /// Write a single char.
  /// </summary>
  /// <param name="ch"></param>
  public override void Write(char ch)
  {
    if (Options.UseCharFunctions)
    {
      if (TryUseCharFunctions(ch))
        return;
    }

    var ctg = Char.GetUnicodeCategory(ch);
    if (ctg == UnicodeCategory.SpaceSeparator || ctg == UnicodeCategory.ParagraphSeparator || ctg == UnicodeCategory.LineSeparator)
      WriteSeparatorChar(ch);
    else
    if (ctg == UnicodeCategory.DashPunctuation || ch == '\u00AD')
      WriteDashOrHyphen(ch);
    else
    if (ctg == UnicodeCategory.Format)
      WriteFormatChar(ch);
    else
    if (ctg == UnicodeCategory.ModifierSymbol || ctg == UnicodeCategory.NonSpacingMark)
      WriteAccentChar(ch);
    else
    if (ctg == UnicodeCategory.Control)
      WriteControlChar(ch);
    else
    if (CharUtils.SupChars.Contains(ch) || CharUtils.SubChars.Contains(ch))
      WriteSupSubChar(ch);
    else
    if (CharUtils.RomanChars.Contains(ch))
      WriteRomanChar(ch);
    else
    {
      WriteOtherChar(ch);
    }
  }

  private bool TryUseCharFunctions(char ch)
  {
    if (CharNames.CharFunctions.TryGetValue(ch, out var seq))
    {
      base.Write("\\" + seq);
      wasBackslash = false;
      return true;
    }
    return false;
  }

  private void WriteControlCharName(string controlCharName)
  {
    base.Write(@"\" + controlCharName);
    wasBackslash = controlCharName.Last() != '}';
  }

  private void WriteCharHex(char ch)
  {
    base.Write($@"\'{((int)ch):X4}");
    wasBackslash = true;
  }

  private void WriteControlChar(char ch)
  {
    if (Options.UseEscapeSequences && CharNames.EscapeMapping.TryGetValue(ch, out var seq))
      WriteControlCharName(seq);
    else
    if (Options.UseControlCharNames && CharNames.TryGetName(ch, out var controlCharName))
      WriteControlCharName(controlCharName);
    else
      WriteCharHex(ch);
  }

  private void WriteSeparatorChar(char ch)
  {
    if (ch == ' ')
      WriteOtherChar(ch);
    else
    if (Options.UseSeparatorNames && CharNames.TryGetName(ch, out var controlCharName))
      WriteControlCharName(controlCharName);
    else
      WriteCharHex(ch);
  }

  private void WriteDashOrHyphen(char ch)
  {
    if (ch == '-')
      WriteOtherChar(ch);
    else
    if (Options.UseDashNames && CharNames.TryGetName(ch, out var controlCharName))
      WriteControlCharName(controlCharName);
    else
      WriteCharHex(ch);
  }

  private void WriteFormatChar(char ch)
  {
    if (Options.UseFormatCharNames && CharNames.TryGetName(ch, out var controlCharName))
      WriteControlCharName(controlCharName);
    else
      WriteCharHex(ch);

  }

  private void WriteAccentChar(char ch)
  {
    if (Options.UseFormatCharNames && CharNames.TryGetName(ch, out var controlCharName))
      WriteControlCharName(controlCharName);
    else
      WriteCharHex(ch);
  }

  private void WriteSupSubChar(char ch)
  {
    if (Options.UseSupSubCharNames && CharNames.TryGetName(ch, out var controlCharName))
      WriteControlCharName(controlCharName);
    else
      WriteCharHex(ch);
  }

  private void WriteRomanChar(char ch)
  {
    if (Options.UseRomanCharNames && CharNames.TryGetName(ch, out var controlCharName))
      WriteControlCharName(controlCharName);
    else
      WriteCharHex(ch);
  }

  private void WriteOtherChar(char ch)
  {
    if (Options.UseOtherCharNames)
    {
      if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
      {
        if (Options.UseAlphanumericCodes)
          WriteCharHex(ch);
        else
          WriteSimpleChar(ch);
      }
      else
      if (CharNames.TryGetName(ch, out var controlCharName))
        WriteControlCharName(controlCharName);
      else
        WriteCharHex(ch);
    }
    else
      WriteSimpleChar(ch);
  }

  private void WriteSimpleChar(char ch)
  {
    if (wasBackslash)
    {
      base.Write(' ');
      wasBackslash = false;
    }
    base.Write(ch);
  }
}
