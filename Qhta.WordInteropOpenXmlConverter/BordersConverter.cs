using System;

using DocumentFormat.OpenXml;
using static Microsoft.Office.Interop.Word.WdLineStyle;
using static Microsoft.Office.Interop.Word.WdLineWidth;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public class BordersConverter()
{
  public W.ParagraphBorders? CreateBorders(Word.Borders borders)
  {
    W.ParagraphBorders? xBorders = new W.ParagraphBorders();
    var hasBorders = false;

    try
    {
      var border = borders[Word.WdBorderType.wdBorderBottom];
      if (border.LineStyle != 0)
      {
        var xBorder = ConvertBorder<W.BottomBorder>(border);
        xBorders.Append(xBorder);
        hasBorders = true;
      }
    }
    catch { }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderTop];
      var ls = border.LineStyle;
      var lw = border.LineWidth;
      if (border.LineStyle != 0)
      {
        var xBorder = ConvertBorder<W.TopBorder>(border);
        xBorders.Append(xBorder);
        hasBorders = true;
      }
    }
    catch { }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderLeft];
      if (border.LineStyle != 0)
      {
        var xBorder = ConvertBorder<W.LeftBorder>(border);
        xBorders.Append(xBorder);
        hasBorders = true;
      }
    }
    catch { }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderRight];
      if (border.LineStyle != 0)
      {
        var xBorder = ConvertBorder<W.RightBorder>(border);
        xBorders.Append(xBorder);
        hasBorders = true;
      }
    }
    catch { }
    if (hasBorders)
      return xBorders;
    return null;
  }

  public BorderType ConvertBorder<BorderType>(Word.Border border) where BorderType : W.BorderType, new()
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xBorder = new BorderType();
    xBorder.Val = new EnumValue<W.BorderValues>(WdBorderToOpenXmlBorder(border.LineStyle));
    var xColor = ConvertColor(border.Color, border.ColorIndex);
    if (xColor != null)
    {
      if (xColor.Val != null)
        xBorder.Color = xColor.Val;
      if (xColor.ThemeColor != null)
        xBorder.ThemeColor = xColor.ThemeColor;
      if (xColor.ThemeShade != null)
        xBorder.ThemeShade = xColor.ThemeShade;
      if (xColor.ThemeTint != null)
        xBorder.ThemeTint = xColor.ThemeTint;
    }
    xBorder.Size = WdLineWidthToBorderWidth(border.LineWidth);
    return xBorder;
  }

  public static UInt32Value? WdLineWidthToBorderWidth(Word.WdLineWidth borderLineWidth)
  {
    return borderLineWidth switch
    {
      wdLineWidth025pt => 12U, // Closest to 0.25 pt
      wdLineWidth050pt => 24U, // Closest to 0.50 pt
      wdLineWidth075pt => 36U, // Closest to 0.75 pt
      wdLineWidth100pt => 48U, // Closest to 1.00 pt
      wdLineWidth150pt => 72U, // Closest to 1.50 pt
      wdLineWidth225pt => 108U, // Closest to 2.25 pt
      wdLineWidth300pt => 144U, // Closest to 3.00 pt
      wdLineWidth450pt => 216U, // Closest to 4.50 pt
      wdLineWidth600pt => 288U, // Closest to 6.00 pt
      _ => null // Add more mappings as needed or return a default value
    };
  }

  public static W.BorderValues WdBorderToOpenXmlBorder(Word.WdLineStyle borderLineStyle)
  {
    return borderLineStyle switch
    {
      wdLineStyleNone => W.BorderValues.Nil,
      wdLineStyleSingle => W.BorderValues.Single,
      wdLineStyleDot => W.BorderValues.Dotted,
      wdLineStyleDashSmallGap => W.BorderValues.DashSmallGap,
      wdLineStyleDashLargeGap => W.BorderValues.Dashed,
      wdLineStyleDashDot => W.BorderValues.DotDash,
      wdLineStyleDashDotDot => W.BorderValues.DotDotDash,
      wdLineStyleDouble => W.BorderValues.Double,
      wdLineStyleTriple => W.BorderValues.Triple,
      wdLineStyleThinThickSmallGap => W.BorderValues.ThinThickThinSmallGap,
      wdLineStyleThickThinSmallGap => W.BorderValues.ThinThickThinSmallGap,
      wdLineStyleThinThickThinSmallGap => W.BorderValues.ThinThickThinSmallGap,
      wdLineStyleThinThickMedGap => W.BorderValues.ThinThickMediumGap,
      wdLineStyleThickThinMedGap => W.BorderValues.ThinThickMediumGap,
      wdLineStyleThinThickThinMedGap => W.BorderValues.ThinThickThinMediumGap,
      wdLineStyleThinThickLargeGap => W.BorderValues.ThickThinLargeGap,
      wdLineStyleThickThinLargeGap => W.BorderValues.ThickThinLargeGap,
      wdLineStyleThinThickThinLargeGap => W.BorderValues.ThinThickThinLargeGap,
      wdLineStyleSingleWavy => W.BorderValues.Wave,
      wdLineStyleDoubleWavy => W.BorderValues.DoubleWave,
      wdLineStyleDashDotStroked => W.BorderValues.DashDotStroked,
      wdLineStyleEmboss3D => W.BorderValues.ThreeDEmboss,
      wdLineStyleEngrave3D => W.BorderValues.ThreeDEngrave,
      wdLineStyleOutset => W.BorderValues.Outset,
      wdLineStyleInset => W.BorderValues.Inset,
      _ => throw new ArgumentOutOfRangeException(nameof(borderLineStyle), borderLineStyle, null)
    };
  }
}
