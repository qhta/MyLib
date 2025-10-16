using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Qhta.WPF.Converters;

/// <summary>
/// Converts an image file path to a grayscale image representation.
/// </summary>
/// <remarks>This converter takes a file path to an image as input and returns a grayscale version of the image.
/// The input value must be a string representing the image file path. If the input is not a valid string or the file
/// path is invalid, the original value is returned unchanged.</remarks>
public class ImageToGrayConverter : IValueConverter
{
  /// <summary>
  /// Converts an image file path to a grayscale image.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is bool isEnabled && parameter is string imagePath)
    {
      var bitmap = new BitmapImage(new Uri(imagePath, UriKind.Relative));
      if (isEnabled)
      {
        return new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
      }
      else
      {
        return ConvertToGrayscaleWithAlpha(bitmap);
      }
    }
    return value;
  }

  /// <summary>
  /// Converts a BitmapSource to grayscale while preserving the alpha channel.
  /// </summary>
  private WriteableBitmap ConvertToGrayscaleWithAlpha(BitmapSource source)
  {
    // Create a WriteableBitmap to manipulate pixel data
    var writableBitmap = new WriteableBitmap(source);

    // Get the pixel data
    int width = writableBitmap.PixelWidth;
    int height = writableBitmap.PixelHeight;
    int stride = width * 4; // BGRA format (4 bytes per pixel)
    byte[] pixelData = new byte[height * stride];

    // Copy the pixel data from the source
    writableBitmap.CopyPixels(pixelData, stride, 0);

    // Iterate through each pixel and convert to grayscale
    for (int i = 0; i < pixelData.Length; i += 4)
    {
      byte blue = pixelData[i];       // Blue channel
      byte green = pixelData[i + 1]; // Green channel
      byte red = pixelData[i + 2];   // Red channel
      byte alpha = pixelData[i + 3]; // Alpha channel

      // Calculate the grayscale value
      byte gray = (byte)(0.3 * red + 0.59 * green + 0.11 * blue);

      // Set the pixel to grayscale while preserving the alpha channel
      pixelData[i] = gray;       // Blue
      pixelData[i + 1] = gray;   // Green
      pixelData[i + 2] = gray;   // Red
      pixelData[i + 3] = alpha;  // Alpha
    }

    // Write the modified pixel data back to the WriteableBitmap
    writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelData, stride, 0);

    return writableBitmap;
  }

  /// <summary>
  /// Not implemented - conversion back is not supported.
  /// </summary>
  /// <exception cref="NotImplementedException"></exception>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}