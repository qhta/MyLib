using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Reflection;
using TA = TAFactory.IconPack;
using System.Xml;
using DWG = System.Drawing;
using System.IO;

namespace MyLib.IconUtils
{
  /// <summary>
  /// Klasa zarządzająca ikonami
  /// </summary>
  public static class IconTools
  {
    private const uint Mega = 1024*1024;

    /// <summary>
    /// Pobranie ikony skojarzonej z plikiem o podanej nazwie
    /// </summary>
    /// <param name="filename">pełna nazwa pliku</param>
    /// <returns></returns>
    public static Iconex GetAssociatedIcon(string filename, IconFromFileArgs args = null)
    {
      filename = filename.ToLower();
      string ext = IO.Path.GetExtension(filename);
      Iconex result = null;
      if (!String.IsNullOrEmpty(ext))
        try
        {
          object fileType = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + ext, "", null);
          if (fileType is string)
          {
            object defaultIcon = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + fileType + @"\DefaultIcon", "", null);
            if (defaultIcon is string)
            {
              if ("%1" == (string)defaultIcon)
              {
                return GetIconFromFile(filename, args);
              }
              else
              {
                string[] ss = (defaultIcon as string).Split(',');
                string appFileName = ss[0];
                int resourceID = 0;
                if (ss.Length > 1)
                  Int32.TryParse(ss[1], out resourceID);
                if (resourceID < 0)
                  resourceID = -resourceID;
                if (IO.Path.GetExtension(appFileName).ToLower() == ".ico")
                  return GetIconFromIcoFile(appFileName, args);
                else
                {
                  result = new Iconex();
                  TA.IconExtractor extractor = new TA.IconExtractor(appFileName);
                  int iconIndex = -1;
                  for (int i = 0; i < extractor.IconNamesList.Count; i++)
                    if (extractor.IconNamesList[i].Id == resourceID)
                    {
                      iconIndex = i;
                      break;
                    }
                  if (iconIndex >= 0)
                  {
                    using (Icon drawingIcon = extractor.GetIconAt(iconIndex))
                      AddImages(drawingIcon, result, args);
                  }
                }
              }
            }
          }
        }
        catch
        {

        }
      if (result == null)
      {
        result = new Iconex();
        string shellFileName = IO.Path.Combine(Environment.GetEnvironmentVariable("windir"), "system32", "shell32.dll");
        TA.IconExtractor extractor = new TA.IconExtractor(shellFileName);
        using (Icon drawingIcon = extractor.GetIconAt(0))
          AddImages(drawingIcon, result, args);
      }
      result.OriginalImageType="EXT";
      return result;
    }

    /// <summary>
    /// Pobranie ikony z pliku graficznego
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static Iconex GetIconFromFile(string filename, IconFromFileArgs args = null)
    {
      if (System.IO.Path.GetExtension(filename).ToLower() == ".ico")
        return GetIconFromIcoFile(filename, args);
      if (System.IO.Path.GetExtension(filename).ToLower() == ".svg")
        return GetIconFromSvgFile(filename, args);
      Iconex result = new Iconex { Name = System.IO.Path.GetFileNameWithoutExtension(filename) };
      var ext = System.IO.Path.GetExtension(filename);
      if (ext.StartsWith("."))
        ext = ext.Remove(0, 1);
      result.OriginalImageType="EXT";
      if (System.IO.Path.GetExtension(filename).ToLower() == ".cdr")
        return result;
      try
      {
        System.Drawing.Bitmap btm = new System.Drawing.Bitmap(filename);
        int size = Math.Max(btm.Size.Width, btm.Size.Height);
        IO.MemoryStream mStream = new IO.MemoryStream();
        if (btm.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        {
          System.Drawing.Bitmap btm1 = btm.Clone(new Rectangle(0, 0, btm.Width, btm.Height), PixelFormat.Format32bppArgb);
          btm.Dispose();
          btm = btm1;
          btm1 = null;
        }
        if (!HasMask(btm))
        {
          Color color0 = btm.GetPixel(0, 0);
          if (btm.GetPixel(btm.Width - 1, btm.Height - 1) == color0
            && btm.GetPixel(0, btm.Height - 1) == color0
            && btm.GetPixel(btm.Width - 1, 0) == color0)
          {
            btm.MakeTransparent(color0);
          }
        }
        AddImages(btm, result, args);
        result.OriginalImagesCount = 1;
      }
      catch { }
      return result;
    }

    public static Iconex FilterIconImages(Iconex source, IconFromFileArgs args)
    {
      Iconex newIcon = (Iconex)source.Clone();
      if (args != null)
      {
        if (args.CreateMissingSizes)
          AddMissingImageSizes(newIcon, args);
        RemoveUnwantedImageSizes(newIcon, args);
      }
      if (args != null)
      {
        if (args.CreateMissingDepths)
          AddMissingImageDepths(newIcon, args);
        RemoveUnwantedImageDepths(newIcon, args);
      }
      return newIcon;
    }

    /// <summary>
    /// Pobranie ikony z pliku ICO
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static Iconex GetIconFromIcoFile(string filename, IconFromFileArgs args)
    {
      Iconex result = new Iconex { Name = System.IO.Path.GetFileNameWithoutExtension(filename) };
      var ext = System.IO.Path.GetExtension(filename);
      if (ext.StartsWith("."))
        ext = ext.Remove(0, 1);
      result.OriginalImageType=ext.ToUpper();
      Icon drawingIcon = new Icon(filename);
      AddImages(drawingIcon, result, args);
      return result;
    }

    /// <summary>
    /// Pobranie ikony z pliku SVG
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static Iconex GetIconFromSvgFile(string filename, IconFromFileArgs args = null)
    {
      Iconex result = new Iconex { Name = System.IO.Path.GetFileNameWithoutExtension(filename) };
      var ext = System.IO.Path.GetExtension(filename);
      if (ext.StartsWith("."))
        ext = ext.Remove(0, 1);
      result.OriginalImageType=ext.ToUpper();
      XmlDocument xmlDoc = new XmlDocument();
      using (XmlReader xmlReader = XmlTextReader.Create(filename, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
      {
        xmlDoc.Load(xmlReader);
        Svg.SvgDocument svgDoc = Svg.SvgDocument.Open(xmlDoc);
        if (args == null)
          args = new IconFromFileArgs { Sizes = new int[] { 16, 24, 32, 48, 64, 256 }, Depths = new ColorDepth[] { ColorDepth.Full } };
        foreach (int size in args.Sizes)
        {
          Bitmap bmp = svgDoc.Draw(size, size);
          result.AddImage(size, ColorDepth.Full, bmp);
        }
      }
      return result;
    }

    /*
  Indexed              = 0x010000,
  Gdi                  = 0x020000,
  Format16bppRgb555    = 0x021005,
  Format16bppRgb565    = 0x021006,
  Format24bppRgb       = 0x021808,
  Format32bppRgb       = 0x022009,
  Format1bppIndexed    = 0x030101,
  Format4bppIndexed    = 0x030402,
  Format8bppIndexed    = 0x030803,
  Alpha                = 0x040000,
  Format16bppArgb1555  = 0x061007,
  PAlpha               = 0x080000,
  Format32bppPArgb     = 0x0E200B,
  Extended             = 0x100000,
  Format16bppGrayScale = 0x101004,
  Format48bppRgb       = 0x10300C,
  Format64bppPArgb     = 0x1C400E,
  Canonical            = 0x200000,
  Format32bppArgb      = 0x26200A,
  Format64bppArgb      = 0x34400D,
    */
    public static int PixelFormat2ColorDepth(PixelFormat format)
    {
      return (((int)format) >> 8) & 0xFF;
    }

    /// <summary>
    /// Dodanie obrazów z bitmapy GDI+ do encji ikony
    /// </summary>
    /// <param name="drawingBitmap">bitmapa GDI+</param>
    /// <param name="icon">encja ikony</param>
    /// <param name="args">opcjonalne określenie, które obrazy dodawać; jeśli brak - to wszystkie</param>
    private static void AddImages(Bitmap drawingBitmap, Iconex icon, IconFromFileArgs args = null)
    {
      AddExistingImage(drawingBitmap, icon);
      if (args != null)
      {
        if (args.CreateMissingSizes)
          AddMissingImageSizes(icon, args);
        RemoveUnwantedImageSizes(icon, args);
      }
      if (args != null)
      {
        if (args.CreateMissingDepths)
          AddMissingImageDepths(icon, args);
        RemoveUnwantedImageDepths(icon, args);
      }
    }

    /// <summary>
    /// Dodanie istniejącego obrazu z bitmapy GDI+ do encji ikony
    /// </summary>
    /// <param name="drawingBitmap">bitmapa GDI+</param>
    /// <param name="icon">encja ikony</param>
    private static void AddExistingImage(Bitmap drawingBitmap, Iconex icon)
    {
      icon.AddImage(Math.Max(drawingBitmap.Width, drawingBitmap.Height), drawingBitmap);
    }

    /// <summary>
    /// Dodanie obrazów z ikony GDI+ do encji ikony
    /// </summary>
    /// <param name="drawingIcon">ikona GDI+</param>
    /// <param name="icon">encja ikony</param>
    /// <param name="args">opcjonalne określenie, które obrazy dodawać; jeśli brak - to wszystkie</param>
    private static void AddImages(Icon drawingIcon, Iconex icon, IconFromFileArgs args = null)
    {
      AddExistingImages(drawingIcon, icon);
      if (args != null)
      {
        if (args.CreateMissingSizes)
          AddMissingImageSizes(icon, args);
        RemoveUnwantedImageSizes(icon, args);
      }
      if (args != null)
      {
        if (args.CreateMissingDepths)
          AddMissingImageDepths(icon, args);
        RemoveUnwantedImageDepths(icon, args);
      }
    }

    /// <summary>
    /// Dodanie istniejących obrazów z ikony GDI+ do encji ikony
    /// </summary>
    /// <param name="drawingIcon">ikona GDI+</param>
    /// <param name="icon">encja ikony</param>
    private static void AddExistingImages(Icon drawingIcon, Iconex icon)
    {
      List<Icon> allSizesIcons = TA.IconHelper.SplitGroupIcon(drawingIcon);
      icon.OriginalImagesCount = allSizesIcons.Count;
      allSizesIcons.Sort(DrawingIconComparison);
      foreach (Icon icn in allSizesIcons)
      {
        IO.MemoryStream ms = new IO.MemoryStream();
        {
          icn.Save(ms);
          ms.Flush();
          ms.Seek(6, IO.SeekOrigin.Begin);
          int width = ms.ReadByte();
          if (width == 0)
            width = 256;
          int height = ms.ReadByte();
          if (height == 0)
            height = 256;
          ms.Seek(12, IO.SeekOrigin.Begin);
          int colorBpp = ms.ReadByte() + (int)ms.ReadByte() * 256;
          int length = ms.ReadByte() + (int)ms.ReadByte() * 256 + (int)ms.ReadByte() * 256 * 256 + (int)ms.ReadByte() * 256 * 256 * 256;
          int offset = ms.ReadByte() + (int)ms.ReadByte() * 256 + (int)ms.ReadByte() * 256 * 256 + (int)ms.ReadByte() * 256 * 256 * 256;
          ms.Seek(offset, IO.SeekOrigin.Begin);
          byte[] image = new byte[ms.Length - offset];
          ms.Read(image, 0, image.Length);
          int size = Math.Max(width, height);
          string format = "BMP";
          if (image.Length > 4 && image[1] == 'P' && image[2] == 'N' && image[3] == 'G')
            format = "PNG";
          Bitmap bmp = new Bitmap(ms, false);
          if (colorBpp == 0)
          {
            if (format == "BMP")
              colorBpp = image[14] + (int)image[15] * 256;
            else
              colorBpp = 32;
          }
          ColorDepth colorDepth = (ColorDepth)(colorBpp);
          if (colorBpp == 256 && IsMonochrome(bmp))
            colorDepth = ColorDepth.Gray;
          if (!icon.HasImage(size, colorDepth))
          {
            IconImage newImage = icon.AddImage(size, colorDepth, bmp);
            newImage.OriginalFormat = GetOriginalFormat(image, format, colorBpp, colorDepth);
          }
        }
      }
    }


    private static string GetOriginalFormat(byte[] image, string format, int colorBpp, ColorDepth colorDepth)
    {
      if (format == "BMP")
      {
        uint colorsCount = image[32] + ((uint)image[33] << 8) + ((uint)image[34] << 16) + ((uint)image[35] << 24);
        if (colorsCount == 0)
          colorsCount = (uint)Math.Pow(2.0, colorBpp);
        if (colorDepth == ColorDepth.Mono)
          return "BMP B/W";
        else if (colorDepth == ColorDepth.Gray)
          return "BMP Gray scale";
        else
        {
          if (colorsCount==0)
            return "BMP full colors";
          else if (colorsCount>=Mega)
            return $"BMP {colorsCount/Mega}M colors";
          else
            return $"BMP {colorsCount} colors";
        }
      }
      else if (format == "PNG")
        return "PNG";
      else
        return "unknown";
    }

    /// <summary>
    /// Skopiowanie żądanych obrazów z jednej ikony do drugiej
    /// </summary>
    /// <param name="allImagesIcon">źródłowa encja ikony zawierająca wszystkie zaprojektowane obrazy</param>
    /// <param name="targetIcon">docelowa encja ikony</param>
    /// <param name="args">które obrazy kopiować</param>
    private static void CopyRequestedImages(Iconex allImagesIcon, Iconex targetIcon, IconFromFileArgs args)
    {
      if (args != null)
        foreach (IconImage image in allImagesIcon.Images)
        {
          if (args.Sizes == null || args.Sizes.Contains(image.Size))
            if (args.Depths == null || args.Depths.Contains(image.ColorDepth))
              targetIcon.Images.Add(image);
        }
    }

    /// <summary>
    /// Dodanie obrazów w brakujących rozmiarach do encji ikony
    /// </summary>
    /// <param name="icon">encja ikony</param>
    /// <param name="args">które obrazy dodawać</param>
    private static void AddMissingImageSizes(Iconex icon, IconFromFileArgs args)
    {
      if (args != null && args.Sizes != null)
        foreach (int size in args.Sizes)
        {
          IconImage image = icon.Images.FirstOrDefault(item => item.Size == size);
          if (image == null)
          {
            image = icon.Images.LastOrDefault(item => item.Size < size);
            if (image == null)
              image = icon.Images.FirstOrDefault(item => item.Size > size);
            if (image == null)
              continue;
            List<IconImage> images = icon.Images.Where(item => item.Size == image.Size).ToList();
            foreach (IconImage item in images)
            {
              if (args.Depths == null || args.Depths.Contains(item.ColorDepth))
              {
                IconImage newImage = new IconImage { Size = size, ColorDepth = item.ColorDepth, AutoGenerated = true };
                newImage.Image = RescaleBitmap(item.Image, item.Size, newImage.Size);
                newImage.OriginalFormat = "PNG";
                icon.Images.Add(newImage);
              }
            }
          }
        }
    }

    /// <summary>
    /// Dodanie obrazów w brakujących głębiach koloru do encji ikony
    /// </summary>
    /// <param name="icon">encja ikony</param>
    /// <param name="args">które obrazy dodawać</param>
    private static void AddMissingImageDepths(Iconex icon, IconFromFileArgs args)
    {
      if (args != null && args.Depths != null)
      {
        int[] sizes = icon.Images.Select(item => item.Size).Distinct().ToArray();
        foreach (int size in sizes)
          foreach (ColorDepth depth in args.Depths)
          {
            IconImage image = icon.Images.FirstOrDefault(item => item.Size == size && item.ColorDepth == depth);
            if (image == null)
            {
              image = icon.Images.FirstOrDefault(item => item.Size == size && item.ColorDepth > depth);
              if (image == null)
                image = icon.Images.LastOrDefault(item => item.Size == size && item.ColorDepth < depth);
              if (image == null)
                continue;
              IconImage newImage = new IconImage { Size = size, ColorDepth = depth, AutoGenerated = true };
              newImage.Image = RecolorBitmap(image.Image, image.ColorDepth, newImage.ColorDepth, args.BWTreshold, args.Gamma);
              newImage.OriginalFormat = "PNG";
              icon.Images.Add(newImage);
            }
          }
      }
    }

    /// <summary>
    /// Usunięcie obrazów w niechcianych rozmiarach z encji ikony
    /// </summary>
    /// <param name="icon">encja ikony</param>
    /// <param name="args">które obrazy są żądane</param>
    private static void RemoveUnwantedImageSizes(Iconex icon, IconFromFileArgs args)
    {
      if (args != null && args.Sizes != null && args.Sizes.Length > 0)
        icon.Images.RemoveWhere(item => !(args.Sizes.Contains(item.Size)));
    }

    /// <summary>
    /// Usunięcie obrazów w niechcianych głębiach koloru z encji ikony
    /// </summary>
    /// <param name="icon">encja ikony</param>
    /// <param name="args">które obrazy są żądane</param>
    private static void RemoveUnwantedImageDepths(Iconex icon, IconFromFileArgs args)
    {
      if (args != null && args.Depths != null && args.Depths.Length > 0)
        icon.Images.RemoveWhere(item => !(args.Depths.Contains(item.ColorDepth)));
    }

    private static bool HasMask(System.Drawing.Bitmap bitmap)
    {
      byte A;
      for (int y = 0; y < bitmap.Height; y++)
        for (int x = 0; x < bitmap.Width; x++)
          if ((A = bitmap.GetPixel(x, y).A) != 255)
            return true;
      return false;
    }

    private static bool AreSame(System.Drawing.Bitmap bitmap1, System.Drawing.Bitmap bitmap2)
    {
      Color C1;
      Color C2;
      for (int y = 0; y < bitmap1.Height; y++)
        for (int x = 0; x < bitmap1.Width; x++)
        {
          C1 = bitmap1.GetPixel(x, y);
          C2 = bitmap2.GetPixel(x, y);
          if (C1 != C2)
            return false;
        }
      return true;
    }


    private static int DrawingIconComparison(Icon icon1, Icon icon2)
    {
      TA.IconInfo info1 = new TA.IconInfo(icon1);
      TA.IconInfo info2 = new TA.IconInfo(icon2);
      if (info1.Width < info2.Width)
        return -1;
      else if (info1.Width > info2.Width)
        return 1;
      else
      {
        if (info1.ColorDepth > info2.ColorDepth)
          return -1;
        else if (info1.ColorDepth < info2.ColorDepth)
          return 1;
        else
          return 0;
      }
    }


    public static Iconex GetIconFromFont(string fontName, char ch, int size, IconFromFontArgs args=null)
    {
      Iconex icon = new Iconex();
      icon.OriginalImageType="FNT";
      if (Char.IsLetterOrDigit(ch))
        icon.Name = new string(ch, 1);
      Bitmap image = GetImageFromFont(fontName, ch, size, args);
      icon.AddImage(size, image);
      return icon;
    }

    public static Iconex GetIconFromFont(string fontName, char ch, int[] sizes, IconFromFontArgs args=null)
    {
      Iconex icon = new Iconex();
      icon.OriginalImageType="FNT";
      if (Char.IsLetterOrDigit(ch))
        icon.Name = new string(ch, 1);
      foreach (int size in sizes)
      {
        Bitmap image = GetImageFromFont(fontName, ch, size, args);
        icon.AddImage(size, image);
      }
      return icon;
    }

    public static Bitmap GetImageFromFont(string fontName, char ch, int size, IconFromFontArgs args = null)
    {
      return GetImageFromFont(fontName, new string(ch, 1), size, args);
    }

    public static Iconex GetIconFromFont(string fontName, string str, int size, IconFromFontArgs args = null)
    {
      Iconex icon = new Iconex();
      icon.Name = str;
      icon.OriginalImageType="FNT";
      Bitmap image = GetImageFromFont(fontName, str, size, args);
      icon.AddImage(size, image);
      return icon;
    }

    public static Iconex GetIconFromFont(string fontName, string str, int[] sizes, IconFromFontArgs args = null)
    {
      Iconex icon = new Iconex();
      icon.Name = str;
      icon.OriginalImageType="FNT";
      foreach (int size in sizes)
      {
        Bitmap image = GetImageFromFont(fontName, str, size, args);
        icon.AddImage(size, image);
      }
      return icon;
    }

    public static Bitmap GetImageFromFont(string fontName, string str, int size, IconFromFontArgs args = null)
    {
      Bitmap bitmap = new Bitmap(size, size);
      {
        Graphics graphics = Graphics.FromImage(bitmap);
        Color color = Color.Black;
        Brush fBrush = Brushes.Black;
        if (args?.Color!=null)
        {
          string s = args.Color;
          if (s.StartsWith("#"))
          {
            if (int.TryParse(s.Substring(1), out int argb))
              color = Color.FromArgb(argb);
           }
          else
            color = Color.FromName(s);
          if (color!=Color.Black)
            fBrush = new SolidBrush(color);
        }
        graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
        StringFormat sf = new StringFormat
        {
          FormatFlags = StringFormatFlags.DisplayFormatControl
          | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces
        };
        float fontSize = size;
        Font aFont = new Font(fontName, (float)fontSize);
        Size textSize = MeasureDisplayString(graphics, str, aFont);

        if (args != null && args.Scaling != null)
        {
          fontSize = args.Scaling(str, fontSize);
          aFont = new Font(fontName, (float)fontSize);
          textSize = MeasureDisplayString(graphics, str, aFont);
        }
        else
        {
          {
            while (fontSize > 4 && (textSize.Width > size))
            {
              fontSize -= 1;
              aFont = new Font(fontName, (float)fontSize);
              textSize = MeasureDisplayString(graphics, str, aFont);
            }
          }
        }

        int textOffset = textSize.Height - aFont.Height;
        int offsetX = (size - textSize.Width) / 2;
        int offsetY = (size - textSize.Height + textOffset) / 2;

        graphics.DrawString(str, aFont, fBrush, offsetX, offsetY, sf);
        if (args?.Envelope!=null)
        {
          Pen pen = new Pen(color);
          Rectangle rect = new Rectangle(0, 0, size-1, size-1);
          switch (args.Envelope.ToLowerInvariant())
          {
            case "circle":
              graphics.DrawEllipse(pen, rect);
              break;
            case "box":
            case "rect":
              graphics.DrawRectangle(pen, rect);
              break;
          }

        }
      }
      return bitmap;
    }

    public static Size MeasureDisplayString(Graphics graphics, string text, Font font)
    {
      System.Drawing.SizeF size = graphics.MeasureString(text, font);
      int width = (int)Math.Ceiling(size.Width);
      int height = (int)Math.Ceiling(size.Height);

      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, graphics);
      System.Drawing.Graphics anagra = System.Drawing.Graphics.FromImage(bitmap);
      int measured_width = width;
      int measured_height = height;

      anagra.Clear(Color.White);
      anagra.DrawString(text + "|", font, Brushes.Black, 0, 0);
      //width - measured_width, -font.Height / 2);

      int x = 0;
      for (x = width - 1; x >= 0; x--)
      {
        measured_width--;
        if (bitmap.GetPixel(x, height / 2).R != 255)
          break;
      }
      for (int y = height - 1; y >= 0; y--)
      {
        if (bitmap.GetPixel(x, y).R != 255)
          break;
        measured_height--;
      }
      for (int y = 0; y <= height - 1; y++)
      {
        if (bitmap.GetPixel(x, y).R != 255)
          break;
        measured_height--;
      }

      return new Size(measured_width, measured_height);
    }

    public static Bitmap RescaleBitmap(Bitmap image, int sourceSize, int targetSize = 0)
    {
      if (targetSize == 0)
        targetSize = sourceSize;
      DWG.Bitmap dwgRescaledBitmap = new DWG.Bitmap(targetSize, targetSize);
      MemoryStream aStream = new MemoryStream();
      image.Save(aStream, ImageFormat.Png);
      aStream.Seek(0, SeekOrigin.Begin);
      MemoryStream bStream = new MemoryStream();
      aStream.Seek(0, SeekOrigin.Begin);
      DWG.Bitmap dwgBitmap = new DWG.Bitmap(aStream);
      {
        {
          DWG.Graphics dwgGraphics = DWG.Graphics.FromImage(dwgRescaledBitmap);
          {
            dwgGraphics.InterpolationMode = DWG.Drawing2D.InterpolationMode.NearestNeighbor;
            dwgGraphics.PixelOffsetMode = DWG.Drawing2D.PixelOffsetMode.Half;
            dwgGraphics.DrawImage(dwgBitmap, new DWG.Rectangle(0, 0, targetSize, targetSize));
          }
        }
      }
      return dwgRescaledBitmap;
    }

    public static Bitmap DrawGrid(Bitmap image, int sourceSize, int targetSize, Pen gridPen = null)
    {
      if (targetSize == 0)
        targetSize = sourceSize;
      MemoryStream aStream = new MemoryStream();
      image.Save(aStream, ImageFormat.Png);
      aStream.Seek(0, SeekOrigin.Begin);
      MemoryStream bStream = new MemoryStream();
      aStream.Seek(0, SeekOrigin.Begin);
      DWG.Bitmap dwgBitmap = new DWG.Bitmap(aStream);
      {
        {
          DWG.Graphics dwgGraphics = DWG.Graphics.FromImage(dwgBitmap);
          {
            dwgGraphics.InterpolationMode = DWG.Drawing2D.InterpolationMode.NearestNeighbor;
            dwgGraphics.PixelOffsetMode = DWG.Drawing2D.PixelOffsetMode.Half;
            if (targetSize / sourceSize > 2)
            {
              if (gridPen == null)
                gridPen = DWG.Pens.LightGray;
              for (int y = 0; y < targetSize; y += targetSize / sourceSize)
                dwgGraphics.DrawLine(gridPen, 0, y, targetSize, y);
              for (int x = 0; x < targetSize; x += targetSize / sourceSize)
                dwgGraphics.DrawLine(gridPen, x, 0, x, targetSize);
            }
          }
        }
      }
      return dwgBitmap;
    }

    public static bool IsMonochrome(Bitmap image)
    {
      for (int y = 0; y < image.Height; y++)
        for (int x = 0; x < image.Width; x++)
        {
          Color pixel = image.GetPixel(x, y);
          if (pixel.A != 0)
          {
            if (pixel.R != pixel.G) return false;
            if (pixel.G != pixel.B) return false;
            if (pixel.B != pixel.R) return false;
          }
        }
      return true;
    }

    public static Bitmap RecolorBitmap(Bitmap image, ColorDepth sourceDepth, ColorDepth targetDepth, double threshold, double gamma)
    {
      MemoryStream aStream = new MemoryStream();
      image.Save(aStream, ImageFormat.Png);
      aStream.Seek(0, SeekOrigin.Begin);
      MemoryStream bStream = new MemoryStream();
      aStream.Seek(0, SeekOrigin.Begin);
      DWG.Bitmap dwgBitmap = new DWG.Bitmap(aStream);
      DWG.Bitmap newBitmap;
      if (targetDepth == ColorDepth.Mono)
      {
        newBitmap = new DWG.Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(newBitmap))
        {
          ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

          ImageAttributes attributes = new ImageAttributes();
          attributes.SetColorMatrix(colorMatrix);
          attributes.SetThreshold((float)threshold);
          attributes.SetGamma((float)gamma);
          g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
        }
      }
      else if (targetDepth == ColorDepth.Gray)
      {
        newBitmap = new DWG.Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(newBitmap))
        {
          ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

          ImageAttributes attributes = new ImageAttributes();
          attributes.SetColorMatrix(colorMatrix);
          attributes.SetGamma((float)gamma);
          g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
        }
      }
      else if (targetDepth == ColorDepth.Idx16)
      {
        newBitmap = new DWG.Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(newBitmap))
        {
          ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

          ImageAttributes attributes = new ImageAttributes();
          attributes.SetColorMatrix(colorMatrix);
          g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
        }
      }
      else if (targetDepth == ColorDepth.Idx256)
      {
        newBitmap = new DWG.Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(newBitmap))
        {
          ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

          ImageAttributes attributes = new ImageAttributes();
          attributes.SetColorMatrix(colorMatrix);
          g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
        }
      }
      else
        newBitmap = dwgBitmap.Clone(new DWG.Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format32bppArgb);
      return newBitmap;
    }

  }
}
