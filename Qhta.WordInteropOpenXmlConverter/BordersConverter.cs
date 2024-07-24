using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;

using static Microsoft.Office.Interop.Word.WdLineStyle;
using static Microsoft.Office.Interop.Word.WdLineWidth;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;

namespace Qhta.WordInteropOpenXmlConverter;

public class BordersConverter
{
  public List<W.BorderType>? CreateBordersList(Word.Borders borders)
  {
    List<W.BorderType> xBorders = new();
    var hasBorders = false;
    //Debug.WriteLine($"borders.Count={borders.Count}");
    try
    {
      var border = borders[Word.WdBorderType.wdBorderTop];
      if (border != null)
      {
        var xBorder = ConvertBorder<W.TopBorder>(border);
        if (xBorder != null)
        {
          xBorders.Add(xBorder);
          hasBorders = true;
        }
      }
    }
    catch (COMException) { }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderBottom];
      if (border != null)
      {
        var xBorder = ConvertBorder<W.BottomBorder>(border);
        if (xBorder != null)
        {
          xBorders.Add(xBorder);
          hasBorders = true;
        }
      }
    }
    catch (COMException) { }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderLeft];
      if (border != null)
      {
        var xBorder = ConvertBorder<W.LeftBorder>(border);
        if (xBorder != null)
        {
          xBorders.Add(xBorder);
          hasBorders = true;
        }
      }
    }
    catch (COMException) { }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderRight];
      if (border != null)
      {
        var xBorder = ConvertBorder<W.RightBorder>(border);
        if (xBorder != null)
        {
          xBorders.Add(xBorder);
          hasBorders = true;
        }
      }
    }
    catch (COMException) { }
    if (borders.HasHorizontal)
    {
      try
      {
        var border = borders[Word.WdBorderType.wdBorderHorizontal];
        if (border != null)
        {
          var xBorder = ConvertBorder<W.InsideHorizontalBorder>(border);
          if (xBorder != null)
          {
            xBorders.Add(xBorder);
            hasBorders = true;
          }
        }
      }
      catch (COMException) { }
    }

    if (borders.HasVertical)
    {
      try
      {
        var border = borders[Word.WdBorderType.wdBorderVertical];
        if (border != null)
        {
          var xBorder = ConvertBorder<W.InsideVerticalBorder>(border);
          if (xBorder != null)
          {
            xBorders.Add(xBorder);
            hasBorders = true;
          }
        }
      }
      catch (COMException) { }
    }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderDiagonalDown];
      if (border != null)
      {
        var xBorder = ConvertBorder<W.TopLeftToBottomRightCellBorder>(border);
        if (xBorder != null)
        {
          xBorders.Add(xBorder);
          hasBorders = true;
        }
      }
    }
    catch (COMException) { }
    try
    {
      var border = borders[Word.WdBorderType.wdBorderDiagonalUp];
      if (border != null)
      {
        var xBorder = ConvertBorder<W.TopRightToBottomLeftCellBorder>(border);
        if (xBorder != null)
        {
          xBorders.Add(xBorder);
          hasBorders = true;
        }
      }
    }
    catch (COMException) { }
    if (hasBorders)
      return xBorders;
    return null;
  }

  public BorderType? ConvertBorder<BorderType>(Word.Border border) where BorderType : W.BorderType, new()
  {
    if (!border.Visible)
      return null;
    var xBorder = new BorderType();
    var borderValue = WdLineStyleToOpenXmlBorder(border.LineStyle);
    if (borderValue == null)
      return null;
    xBorder.Val = new EnumValue<W.BorderValues>(borderValue);
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

  public static W.BorderValues? WdLineStyleToOpenXmlBorder(Word.WdLineStyle borderLineStyle)
  {
    if ((int)borderLineStyle == wdUndefined)
      return null;
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
