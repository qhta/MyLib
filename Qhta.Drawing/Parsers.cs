using System.Drawing;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Markup;

namespace System.Drawing
{
  internal static class Parsers
  {
    private const int s_zeroChar = 0x30;
    private const int s_aLower = 0x61;
    private const int s_aUpper = 0x41;
    internal const string s_ContextColor = "ContextColor ";
    internal const string s_ContextColorNoSpace = "ContextColor";

    //internal static object DeserializeStreamGeometry(BinaryReader reader)
    //{
    //  StreamGeometry geometry = new StreamGeometry();
    //  using (StreamGeometryContext context = geometry.Open())
    //  {
    //    ParserStreamGeometryContext.Deserialize(reader, context, geometry);
    //  }
    //  geometry.Freeze();
    //  return geometry;
    //}

    internal static Brush ParseBrush(string brush, IFormatProvider formatProvider, ITypeDescriptorContext context)
    {
      bool flag;
      bool flag2;
      bool flag3;
      bool flag4;
      string trimmedColor = KnownColors.MatchColor(brush, out flag4, out flag3, out flag2, out flag);
      if (trimmedColor.Length == 0)
      {
        throw new FormatException(SR.Get("Parser_Empty"));
      }
      if (flag3)
      {
        return new SolidBrush(ParseHexColor(trimmedColor));
      }
      if (flag2)
      {
        return new SolidBrush(ParseContextColor(trimmedColor, formatProvider, context));
      }
      if (flag)
      {
        return new SolidBrush(ParseScRgbColor(trimmedColor, formatProvider));
      }
      if (flag4)
      {
        SolidBrush brush2 = KnownColors.ColorStringToKnownBrush(trimmedColor);
        if (brush2 != null)
        {
          return brush2;
        }
      }
      throw new FormatException(SR.Get("Parsers_IllegalToken"));
    }

    internal static Color ParseColor(string color, IFormatProvider formatProvider)
    {
      return ParseColor(color, formatProvider, null);
    }

    internal static Color ParseColor(string color, IFormatProvider formatProvider, ITypeDescriptorContext context)
    {
      bool flag;
      bool flag2;
      bool flag3;
      bool flag4;
      string trimmedColor = KnownColors.MatchColor(color, out flag4, out flag3, out flag2, out flag);
      if ((!flag4 && (!flag3 && !flag)) && !flag2)
      {
        throw new FormatException(SR.Get("Parsers_IllegalToken"));
      }
      if (flag3)
      {
        return ParseHexColor(trimmedColor);
      }
      if (flag2)
      {
        return ParseContextColor(trimmedColor, formatProvider, context);
      }
      if (flag)
      {
        return ParseScRgbColor(trimmedColor, formatProvider);
      }
      KnownColor color2 = KnownColors.ColorStringToKnownColor(trimmedColor);
      if (color2 == KnownColor.UnknownColor)
      {
        throw new FormatException(SR.Get("Parsers_IllegalToken"));
      }
      return Color.FromArgb((int)color2);
    }

    private static Color ParseContextColor(string trimmedColor, IFormatProvider formatProvider, ITypeDescriptorContext context)
    {
      if (!trimmedColor.StartsWith("ContextColor ", StringComparison.OrdinalIgnoreCase))
      {
        throw new FormatException(SR.Get("Parsers_IllegalToken"));
      }
      string str = trimmedColor.Substring("ContextColor ".Length).Trim();
      char[] separator = new char[] { ' ' };
      string[] strArray = str.Split(separator);
      if (strArray.GetLength(0) < 2)
      {
        throw new FormatException(SR.Get("Parsers_IllegalToken"));
      }
      str = str.Substring(strArray[0].Length);
      TokenizerHelper helper = new TokenizerHelper(str, formatProvider);
      char[] chArray2 = new char[] { ',', ' ' };
      int length = str.Split(chArray2, StringSplitOptions.RemoveEmptyEntries).GetLength(0);
      float a = Convert.ToSingle(helper.NextTokenRequired(), formatProvider);
      float[] values = new float[3];//[length - 1];
      for (int i = 0; i < (length - 1); i++)
      {
        values[i] = Convert.ToSingle(helper.NextTokenRequired(), formatProvider);
      }
      string inputString = strArray[0];
      //UriHolder uriFromUriContext = System.Windows.Media.TypeConverterHelper.GetUriFromUriContext(context, inputString);
      //Uri profileUri = (uriFromUriContext.BaseUri == null) ? uriFromUriContext.OriginalUri : new Uri(uriFromUriContext.BaseUri, uriFromUriContext.OriginalUri);
      Color color = Color.FromArgb((byte)(a/255), (byte)(values[0]/255), (byte)(values[1]/255), (byte)(values[2]/255));
      return color;
    }

    //internal static Geometry ParseGeometry(string pathString, IFormatProvider formatProvider)
    //{
    //  FillRule evenOdd = FillRule.EvenOdd;
    //  StreamGeometry geometry = new StreamGeometry();
    //  ParseStringToStreamGeometryContext(geometry.Open(), pathString, formatProvider, ref evenOdd);
    //  geometry.FillRule = evenOdd;
    //  geometry.Freeze();
    //  return geometry;
    //}

    private static int ParseHexChar(char c)
    {
      int num = c;
      if ((num >= 0x30) && (num <= 0x39))
      {
        return (num - 0x30);
      }
      if ((num >= 0x61) && (num <= 0x66))
      {
        return ((num - 0x61) + 10);
      }
      if ((num < 0x41) || (num > 70))
      {
        throw new FormatException(SR.Get("Parsers_IllegalToken"));
      }
      return ((num - 0x41) + 10);
    }

    private static Color ParseHexColor(string trimmedColor)
    {
      int num;
      int num2;
      int num3;
      int num4 = 0xff;
      if (trimmedColor.Length > 7)
      {
        num4 = (ParseHexChar(trimmedColor[1]) * 0x10) + ParseHexChar(trimmedColor[2]);
        num3 = (ParseHexChar(trimmedColor[3]) * 0x10) + ParseHexChar(trimmedColor[4]);
        num2 = (ParseHexChar(trimmedColor[5]) * 0x10) + ParseHexChar(trimmedColor[6]);
        num = (ParseHexChar(trimmedColor[7]) * 0x10) + ParseHexChar(trimmedColor[8]);
      }
      else if (trimmedColor.Length > 5)
      {
        num3 = (ParseHexChar(trimmedColor[1]) * 0x10) + ParseHexChar(trimmedColor[2]);
        num2 = (ParseHexChar(trimmedColor[3]) * 0x10) + ParseHexChar(trimmedColor[4]);
        num = (ParseHexChar(trimmedColor[5]) * 0x10) + ParseHexChar(trimmedColor[6]);
      }
      else if (trimmedColor.Length <= 4)
      {
        num3 = ParseHexChar(trimmedColor[1]);
        num3 += num3 * 0x10;
        num2 = ParseHexChar(trimmedColor[2]);
        num2 += num2 * 0x10;
        num = ParseHexChar(trimmedColor[3]);
        num += num * 0x10;
      }
      else
      {
        num4 = ParseHexChar(trimmedColor[1]);
        num4 += num4 * 0x10;
        num3 = ParseHexChar(trimmedColor[2]);
        num3 += num3 * 0x10;
        num2 = ParseHexChar(trimmedColor[3]);
        num2 += num2 * 0x10;
        num = ParseHexChar(trimmedColor[4]);
        num += num * 0x10;
      }
      return Color.FromArgb((byte)num4, (byte)num3, (byte)num2, (byte)num);
    }

    //internal static PathFigureCollection ParsePathFigureCollection(string pathString, IFormatProvider formatProvider)
    //{
    //  PathStreamGeometryContext context = new PathStreamGeometryContext();
    //  new AbbreviatedGeometryParser().ParseToGeometryContext(context, pathString, 0);
    //  return context.GetPathGeometry().Figures;
    //}

    private static Color ParseScRgbColor(string trimmedColor, IFormatProvider formatProvider)
    {
      if (!trimmedColor.StartsWith("sc#", StringComparison.Ordinal))
      {
        throw new FormatException(SR.Get("Parsers_IllegalToken"));
      }
      TokenizerHelper helper = new TokenizerHelper(trimmedColor.Substring(3, trimmedColor.Length - 3), formatProvider);
      float[] numArray = new float[4];
      for (int i = 0; i < 3; i++)
      {
        numArray[i] = Convert.ToSingle(helper.NextTokenRequired(), formatProvider);
      }
      if (!helper.NextToken())
      {
        return Color.FromArgb(255, (byte)(numArray[0]*255), (byte)(numArray[1]*255), (byte)(numArray[2]*255));
      }
      numArray[3] = Convert.ToSingle(helper.GetCurrentToken(), formatProvider);
      if (helper.NextToken())
      {
        throw new FormatException(SR.Get("Parsers_IllegalToken"));
      }
      return Color.FromArgb((byte)(numArray[0]*255), (byte)(numArray[1]*255), (byte)(numArray[2]*255), (byte)(numArray[3]*255));
    }

    //private static void ParseStringToStreamGeometryContext(StreamGeometryContext context, string pathString, IFormatProvider formatProvider, ref FillRule fillRule)
    //{
    //  using (context)
    //  {
    //    if (pathString != null)
    //    {
    //      int index = 0;
    //      while (true)
    //      {
    //        if ((index < pathString.Length) && char.IsWhiteSpace(pathString, index))
    //        {
    //          index++;
    //          continue;
    //        }
    //        if ((index < pathString.Length) && (pathString[index] == 'F'))
    //        {
    //          index++;
    //          while (true)
    //          {
    //            if ((index < pathString.Length) && char.IsWhiteSpace(pathString, index))
    //            {
    //              index++;
    //              continue;
    //            }
    //            if (index == pathString.Length)
    //            {
    //              goto TR_0003;
    //            }
    //            else
    //            {
    //              if ((pathString[index] != '0') && (pathString[index] != '1'))
    //              {
    //                goto TR_0003;
    //              }
    //              fillRule = (pathString[index] == '0') ? FillRule.EvenOdd : FillRule.Nonzero;
    //              index++;
    //            }
    //            break;
    //          }
    //        }
    //        new AbbreviatedGeometryParser().ParseToGeometryContext(context, pathString, index);
    //        break;
    //      }
    //    }
    //    return;
    //    TR_0003:
    //    throw new FormatException(SR.Get("Parsers_IllegalToken"));
    //  }
    //}

    //internal static Transform ParseTransform(string transformString, IFormatProvider formatProvider)
    //{
    //  return new MatrixTransform(Matrix.Parse(transformString));
    //}

    //internal static void PathMinilanguageToBinary(BinaryWriter bw, string stringValue)
    //{
    //  ParserStreamGeometryContext context = new ParserStreamGeometryContext(bw);
    //  FillRule evenOdd = FillRule.EvenOdd;
    //  ParseStringToStreamGeometryContext(context, stringValue, System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS, ref evenOdd);
    //  context.SetFillRule(evenOdd);
    //  context.MarkEOF();
    //}
  }
}

