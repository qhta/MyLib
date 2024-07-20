using System;

using DocumentFormat.OpenXml;

using Microsoft.Office.Interop.Word;

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
      var border = borders[WdBorderType.wdBorderBottom];
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
      var border = borders[WdBorderType.wdBorderTop];
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
      var border = borders[WdBorderType.wdBorderLeft];
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
      var border = borders[WdBorderType.wdBorderRight];
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

  public static UInt32Value? WdLineWidthToBorderWidth(WdLineWidth borderLineWidth)
  {
    return borderLineWidth switch
    {
      WdLineWidth.wdLineWidth025pt => 12U, // Closest to 0.25 pt
      WdLineWidth.wdLineWidth050pt => 24U, // Closest to 0.50 pt
      WdLineWidth.wdLineWidth075pt => 36U, // Closest to 0.75 pt
      WdLineWidth.wdLineWidth100pt => 48U, // Closest to 1.00 pt
      WdLineWidth.wdLineWidth150pt => 72U, // Closest to 1.50 pt
      WdLineWidth.wdLineWidth225pt => 108U, // Closest to 2.25 pt
      WdLineWidth.wdLineWidth300pt => 144U, // Closest to 3.00 pt
      WdLineWidth.wdLineWidth450pt => 216U, // Closest to 4.50 pt
      WdLineWidth.wdLineWidth600pt => 288U, // Closest to 6.00 pt
      _ => null // Add more mappings as needed or return a default value
    };
  }

  public static W.BorderValues WdBorderToOpenXmlBorder(WdLineStyle borderLineStyle)
  {
    return borderLineStyle switch
    {
      WdLineStyle.wdLineStyleNone => W.BorderValues.Nil,
      WdLineStyle.wdLineStyleSingle => W.BorderValues.Single,
      WdLineStyle.wdLineStyleDot => W.BorderValues.Dotted,
      WdLineStyle.wdLineStyleDashSmallGap => W.BorderValues.DashSmallGap,
      WdLineStyle.wdLineStyleDashLargeGap => W.BorderValues.Dashed,
      WdLineStyle.wdLineStyleDashDot => W.BorderValues.DotDash,
      WdLineStyle.wdLineStyleDashDotDot => W.BorderValues.DotDotDash,
      WdLineStyle.wdLineStyleDouble => W.BorderValues.Double,
      WdLineStyle.wdLineStyleTriple => W.BorderValues.Triple,
      WdLineStyle.wdLineStyleThinThickSmallGap => W.BorderValues.ThinThickThinSmallGap,
      WdLineStyle.wdLineStyleThickThinSmallGap => W.BorderValues.ThinThickThinSmallGap,
      WdLineStyle.wdLineStyleThinThickThinSmallGap => W.BorderValues.ThinThickThinSmallGap,
      WdLineStyle.wdLineStyleThinThickMedGap => W.BorderValues.ThinThickMediumGap,
      WdLineStyle.wdLineStyleThickThinMedGap => W.BorderValues.ThinThickMediumGap,
      WdLineStyle.wdLineStyleThinThickThinMedGap => W.BorderValues.ThinThickThinMediumGap,
      WdLineStyle.wdLineStyleThinThickLargeGap => W.BorderValues.ThickThinLargeGap,
      WdLineStyle.wdLineStyleThickThinLargeGap => W.BorderValues.ThickThinLargeGap,
      WdLineStyle.wdLineStyleThinThickThinLargeGap => W.BorderValues.ThinThickThinLargeGap,
      WdLineStyle.wdLineStyleSingleWavy => W.BorderValues.Wave,
      WdLineStyle.wdLineStyleDoubleWavy => W.BorderValues.DoubleWave,
      WdLineStyle.wdLineStyleDashDotStroked => W.BorderValues.DashDotStroked,
      WdLineStyle.wdLineStyleEmboss3D => W.BorderValues.ThreeDEmboss,
      WdLineStyle.wdLineStyleEngrave3D => W.BorderValues.ThreeDEngrave,
      WdLineStyle.wdLineStyleOutset => W.BorderValues.Outset,
      WdLineStyle.wdLineStyleInset => W.BorderValues.Inset,
      _ => throw new ArgumentOutOfRangeException(nameof(borderLineStyle), borderLineStyle, null)
    };
  }
}
