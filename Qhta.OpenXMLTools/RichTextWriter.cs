using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

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
  /// Write a single char.
  /// </summary>
  /// <param name="ch"></param>
  public override void Write(char ch)
  {
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
      if (wasBackslash)
      {
        base.Write(' ');
        wasBackslash = false;
      }
      base.Write(ch);
    }
  }

  private void WriteControlChar(char ch)
  {
    if (Options.UseEscapeSequences && CharNames.EscapeMapping.TryGetValue(ch, out var seq))
      base.Write("\\"+seq);
    else
    if (Options.UseControlCharNames && CharNames.TryGetValue(ch, out var controlCharName))
      base.Write(@"\" + controlCharName);
    else
      base.Write($@"\u{((int)ch):X4}");
    wasBackslash = true;
  }

  private void WriteSeparatorChar(char ch)
  {
    if (ch == ' ')
    {
      base.Write(ch);
      wasBackslash = false;
      return;
    }
    if (Options.UseSpaceNames && CharNames.TryGetValue(ch, out var controlCharName))
      base.Write(@"\" + controlCharName);
    else
      base.Write($@"\u{((int)ch):X4}");
    wasBackslash = true;
  }

  private void WriteDashOrHyphen(char ch)
  {
    if (ch == '-')
    {
      base.Write(ch);
      wasBackslash = false;
      return;
    }
    if (Options.UseDashNames && CharNames.TryGetValue(ch, out var controlCharName))
      base.Write(@"\" + controlCharName);
    else
      base.Write($@"\u{((int)ch):X4}");
    wasBackslash = true;
  }

  private void WriteFormatChar(char ch)
  {
    if (Options.UseFormatCharNames && CharNames.TryGetValue(ch, out var controlCharName))
      base.Write(@"\" + controlCharName);
    else
      base.Write($@"\u{((int)ch):X4}");
    wasBackslash = true;

  }

  private void WriteAccentChar(char ch)
  {
    if (Options.UseFormatCharNames && CharNames.TryGetValue(ch, out var controlCharName))
      base.Write(@"\" + controlCharName);
    else
      base.Write($@"\u{((int)ch):X4}");
    wasBackslash = true;
  }

  private void WriteSupSubChar(char ch)
  {
    if (Options.UseSupSubCharNames && CharNames.TryGetValue(ch, out var controlCharName))
    {
      base.Write(@"\" + controlCharName);
      wasBackslash = controlCharName.Last() != '}';
    }
    else
    {
      base.Write($@"\u{((int)ch):X4}");
      wasBackslash = true;
    }
  }

  private void WriteRomanChar(char ch)
  {
    if (Options.UseRomanCharNames && CharNames.TryGetValue(ch, out var controlCharName))
      base.Write(@"\" + controlCharName);
    else
      base.Write($@"\u{((int)ch):X4}");
    wasBackslash = true;
  }

}
