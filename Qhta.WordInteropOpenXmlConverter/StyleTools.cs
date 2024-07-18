#nullable enable

using System.Collections.Generic;
using Microsoft.Office.Interop.Word;

using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter
{
  public class StyleTools
  {

    public Dictionary<string, WdBuiltinStyle> BuildInStyleNumbers = new();
    public Dictionary<WdBuiltinStyle, string> BuildInStyleNames = new();

    public Word.Document ActiveDocument { get; private set; }

    public StyleTools(Word.Document activeDocument)
    {
      ActiveDocument = activeDocument;
      foreach (Word.WdBuiltinStyle styleNumber in System.Enum.GetValues(typeof(WdBuiltinStyle)))
      {
        var style = GetStyle(styleNumber);

        BuildInStyleNumbers.Add(style.NameLocal, styleNumber);
        BuildInStyleNames.Add(styleNumber, style.NameLocal);
      }
    }

    public Style GetStyle(WdBuiltinStyle styleNumber)
    {
      var document = ActiveDocument;
      return GetStyle(document, styleNumber);
    }

    public Style GetStyle(Document document, WdBuiltinStyle styleNumber)
    {
      return document.Styles[styleNumber];
    }

    public Style? GetStyle(string styleName)
    {
      var document = ActiveDocument;
      return GetStyle(document, styleName);
    }

    public Style? GetStyle(Document document, string styleName)
    {
      return document.Styles[styleName];
    }

    public void SetStyle(Range range, Style style)
    {
      range.set_Style(style);
    }

    public void SetStyle(Range range, WdBuiltinStyle styleNumber)
    {
      range.set_Style(styleNumber);
    }

    public void SetStyle(Range range, string styleName)
    {
      range.set_Style(styleName);
    }

    public void SetStyle(Paragraph para, Style style)
    {
      para.set_Style(style);
    }

    public void SetStyle(Paragraph para, WdBuiltinStyle styleNumber)
    {
      para.set_Style(styleNumber);
    }

    public void SetStyle( Paragraph para, string styleName)
    {
      para.set_Style(styleName);
    }

    public void SetStyle( Table table, Style style)
    {
      table.set_Style(style);
    }

    public void SetStyle( Table table, WdBuiltinStyle styleNumber)
    {
      table.set_Style(styleNumber);
    }

    public void SetStyle( Table table, string styleName)
    {
      table.set_Style(styleName);
    }

    public Style GetStyle( Range range)
    {
      return (Style)range.get_Style();
    }

    public Style GetStyle( Paragraph para)
    {
      return (Style)para.get_Style();
    }

    public Style GetStyle( Table table)
    {
      return (Style)table.get_Style();
    }

    public bool Is( Style style, string styleName)
    {
      return style.NameLocal == styleName;
    }

    public bool Is( Style style, WdBuiltinStyle styleNumber)
    {
      return BuildInStyleNumbers.TryGetValue(style.NameLocal, out var number) && number == styleNumber;
    }

  }
}
