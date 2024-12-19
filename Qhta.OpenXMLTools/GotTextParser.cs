using System;
using System.Runtime.CompilerServices;

namespace Qhta.OpenXmlTools;

/// <summary>
/// This class parses text which was got with PlainText methods for OpenXml elements.
/// </summary>
public class GotTextParser : IDisposable
{

  /// <summary>
  /// Element which text will be parsed.
  /// </summary>
  public DX.OpenXmlElement RootElement { get; }

  /// <summary>
  /// Options used when text was taken from RootElement using PlainText method.
  /// </summary>
  public TextOptions Options { get; }

  /// <summary>
  /// Message with error description if text was not parsed successfully.
  /// </summary>
  public string? ErrorMessage { get; private set; }


  private record TagInfo(string StartTag, string EndTag)
  {
    public string StartTag { get; } = StartTag;
    public string EndTag { get; } = EndTag;
  }

  private readonly Dictionary<string, string> startEndTagDictionary;

  /// <summary>
  /// Constructor. Root element is the element which text will be parsed.
  /// </summary>
  /// <param name="rootElement">Element which text will be parsed.</param>
  /// <param name="options">Options used when text was taken from RootElement using PlainText method.</param>
  public GotTextParser(DX.OpenXmlElement rootElement, TextOptions options)
  {
    RootElement = rootElement;
    Options = options;
    startEndTagDictionary = new Dictionary<string, string>()
    {
      { Options.TextStartTag, Options.TextEndTag},
      { Options.RunStartTag, Options.RunEndTag},
      { Options.ParagraphStartTag, Options.ParagraphEndTag},
      { Options.TableStartTag, Options.TableEndTag},
      { Options.TableRowStartTag, Options.TableRowEndTag},
      { Options.TableCellStartTag, Options.TableCellEndTag},

      { Options.DrawingStartTag, Options.DrawingEndTag},
    };
  }

  /// <summary>
  /// Main method. Parses text. Returns true if text was parsed successfully.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  public bool ParseText(string text)
  {
    int pos = 0;
    return ParseText(RootElement, text, ref pos);
  }

  /// <summary>
  /// Now implementation is empty
  /// </summary>
  public void Dispose()
  {

  }

  /// <summary>
  /// Set text to a Run element.
  /// </summary>
  /// <param name="rootElement"></param>
  /// <param name="text"></param>
  /// <param name="pos">current parse position</param>
  private bool ParseText(DX.OpenXmlElement rootElement, string text, ref int pos)
  {
    var member = rootElement.GetMembers().FirstOrDefault();
    while (pos < text.Length)
    {
      var ch = text[pos];
      if (ch == TextOptions.EscChar)
      {
        pos++;
        if (pos >= text.Length)
          break;
        ch = text[pos];
        if (ch == TextOptions.EscChar)
        {
          if (rootElement is DXW.Text runText)
          {
            if (!runText.SetTextAt(pos, TextOptions.EscChar.ToString(), Options))
            {
              ErrorMessage = $"Can't set Escape character to the run SearchText element holding \"{runText.Text}\" at position {pos}";
              return false;
            }
            pos++;
          }
          if (rootElement is DXW.Run run)
          {
            if (!run.SetTextTo(member, TextOptions.EscChar.ToString(), Options))
            {
              ErrorMessage = $"Can't set Escape character to the Run SearchText at position {pos}";
              return false;
            }
            pos++;
          }
          else
          {
            ErrorMessage = $"Double Escape character is invalid in the context of {rootElement.GetType()} element";
            return false;
          }
        }
        else if (ch == '<')
        {
          var k = text.IndexOf('>', pos);
          if (k < 0)
            return false;
          var tag = text.Substring(pos, k - pos + 1);
          if (startEndTagDictionary.TryGetValue(tag, out var endTag))
          {
            var endPos = text.IndexOf(endTag, pos);
            if (endPos == -1)
            {
              ErrorMessage = $"End tag {endTag} not found ";
              return false;
            }
          }
        }
      }
      else
      {
        var endPos = text.IndexOf(TextOptions.EscChar+"<", pos);
        if (endPos < 0)
          endPos = text.Length;
        var t = text.Substring(pos, endPos - pos);
        if (rootElement is DXW.Text runText)
        {
          if (!runText.SetTextAt(pos, t, Options))
          {
            ErrorMessage = $"Can't set text to the run SearchText element holding \"{runText.Text}\" at position {pos}";
            return false;
          }
        }
        pos = endPos;
      }
    }
    return true;
  }
};

