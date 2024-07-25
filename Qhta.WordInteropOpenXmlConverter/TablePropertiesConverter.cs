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
      if (tableStyle.Spacing != 0)
      {
        var spacing = PointsToTwips(tableStyle.Spacing);
        xTableProperties.TableCellSpacing = new W.TableCellSpacing { Width = spacing.ToString(), Type = W.TableWidthUnitValues.Dxa };
      }

      if (tableStyle.RowStripe>0)
      {
        xTableProperties.TableStyleRowBandSize = new W.TableStyleRowBandSize { Val = tableStyle.RowStripe };
      }

      if (tableStyle.ColumnStripe>0)
      {
        xTableProperties.TableStyleColumnBandSize = new W.TableStyleColumnBandSize { Val = tableStyle.ColumnStripe };
      }

      var cellMargins = CreateCellMargins(tableStyle);
      if (cellMargins != null)
        xTableProperties.TableCellMarginDefault = cellMargins;
      //var allowBreakAcrossPage = tableStyle.AllowBreakAcrossPage;
      //var allowPageBreaks = tableStyle.AllowPageBreaks;
      var borderList = new BordersConverter().CreateBordersList(tableStyle.Borders);
      if (borderList != null)
        xTableProperties.TableBorders = borderList.ToOpenXmlBorders<W.TableBorders>();
    }
    return xTableProperties;
  }

  private W.TableCellMarginDefault? CreateCellMargins(Word.TableStyle tableStyle)
  {
    var cellMargins = new W.TableCellMarginDefault();
    var hasCellMargins = false;
    var leftMargin = GetTwipsValue(tableStyle.LeftPadding);
    if (leftMargin != null)
    {
      cellMargins.StartMargin = new W.StartMargin { Width = leftMargin.ToString(), Type = W.TableWidthUnitValues.Dxa };
      hasCellMargins = true;
    }
    var rightMargin = GetTwipsValue(tableStyle.RightPadding);
    if (rightMargin != null)
    {
      cellMargins.EndMargin = new W.EndMargin { Width = rightMargin.ToString(), Type = W.TableWidthUnitValues.Dxa };
      hasCellMargins = true;
    }
    var topMargin = GetTwipsValue(tableStyle.TopPadding);
    if (topMargin != null)
    {
      cellMargins.TopMargin = new W.TopMargin { Width = topMargin.ToString(), Type = W.TableWidthUnitValues.Dxa };
      hasCellMargins = true;
    }
    var bottomMargin = GetTwipsValue(tableStyle.BottomPadding);
    if (bottomMargin != null)
    {
      cellMargins.BottomMargin = new W.BottomMargin { Width = bottomMargin.ToString(), Type = W.TableWidthUnitValues.Dxa };
      hasCellMargins = true;
    }
    if (!hasCellMargins)
      return null;
    return cellMargins;
  }
}
