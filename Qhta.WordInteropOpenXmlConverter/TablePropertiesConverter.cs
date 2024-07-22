using System;

using DocumentFormat.OpenXml;

using Qhta.OpenXmlTools;

using static Microsoft.Office.Interop.Word.WdLineSpacing;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.PropertiesConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public class TablePropertiesConverter
{
  private readonly Word.ParagraphFormat? defaultParagraph;

  public TablePropertiesConverter()
  {
  }

  public TablePropertiesConverter(Word.Style defaultStyle)
  {
    defaultParagraph = defaultStyle.ParagraphFormat;
  }

  public W.StyleTableProperties ConvertTableProperties(Word.Style wordStyle)
  {
    var tableStyle = wordStyle.Table;
    var xTableProperties = ConvertTableProperties(tableStyle);

    return xTableProperties;
  }

  public W.StyleTableProperties ConvertTableProperties(Word.TableStyle tableStyle)
  {

    var xTableProperties = new W.StyleTableProperties();

    if ((int)tableStyle.Alignment != wdUndefined)
    {
      W.TableRowAlignmentValues alignmentValues = tableStyle.Alignment switch
      {
        Word.WdRowAlignment.wdAlignRowLeft => W.TableRowAlignmentValues.Left,
        Word.WdRowAlignment.wdAlignRowCenter => W.TableRowAlignmentValues.Center,
        Word.WdRowAlignment.wdAlignRowRight => W.TableRowAlignmentValues.Right,
        _ => W.TableRowAlignmentValues.Left
      };
      xTableProperties.TableJustification = new W.TableJustification { Val = alignmentValues };

      if (tableStyle.LeftIndent != 0)
      {
        var leftIndent = PointsToTwips(tableStyle.LeftIndent);
        xTableProperties.TableIndentation = new W.TableIndentation { Width = leftIndent, Type = W.TableWidthUnitValues.Dxa };
      }

      //var allowBreakAcrossPage = tableStyle.AllowBreakAcrossPage;
      //var allowPageBreaks = tableStyle.AllowPageBreaks;
      var borderList = new BordersConverter().CreateBordersList(tableStyle.Borders);
      if (borderList != null)
        xTableProperties.TableBorders = borderList.ToTableBorders();
    }
    return xTableProperties;
  }
}
