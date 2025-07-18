﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// One-way converter that changes Image basing on bool value.
  /// If value is false (not enabled) than Overlay is added to the Image.
  /// </summary>
  public class IconEnabledConverter: IValueConverter
  {
    /// <summary>
    /// If value is false (not enabled) than Overlay is added to the Image.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value!=null && Image!=null)
      {
        if (bool.TryParse(value.ToString(), out var enabled))
        {
          if (!enabled)
          {
            int h = Image.PixelHeight;
            int w = Image.PixelWidth;
            var modifiedImage = new WriteableBitmap(w, h, Image.DpiX, Image.DpiY, PixelFormats.Bgra32, null);
            int[] pixelData = new int[w * h];
            int widthInByte = (int)Math.Ceiling(Image.Format.BitsPerPixel * w /8.0);
            Image.CopyPixels(pixelData, widthInByte, 0);
            if (Overlay!=null && Overlay.PixelHeight==h && Overlay.PixelWidth==w)
            {
              int[] overlayData = new int[w * h];
              Overlay.CopyPixels(overlayData, widthInByte, 0);
              for (int i = 0; i < pixelData.Length; i++)
              {
                var aColor = Color.FromArgb(overlayData[i]);
                if (aColor.A>0)
                {
                  pixelData[i] = aColor.ToArgb();
                }
              }
            }
            else
            {
              for (int i = 0; i < pixelData.Length; i++)
              {
                var aColor = Color.FromArgb(pixelData[i]);
                if (aColor.A>0)
                {
                  aColor =
                    Color.FromArgb(aColor.A, Math.Min(255, aColor.R+0xCF), Math.Min(255, aColor.G+0xCF), Math.Min(255, aColor.B+0xCF));
                  pixelData[i] = aColor.ToArgb();
                }
              }
            }
            modifiedImage.WritePixels(new Int32Rect(0, 0, w, h), pixelData, widthInByte, 0);
            return modifiedImage;
          }
        }
      }
      return Image;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Modified Image.
    /// </summary>
    public BitmapSource Image { get; set; } = null!;

    /// <summary>
    /// Overlay added to Image.
    /// </summary>
    public BitmapSource Overlay { get; set; } = null!;
  }
}
