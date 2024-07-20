using System.Drawing;

namespace Qhta.WordInteropOpenXmlConverter;

public static class NumberConverter
{
  public static float CentimetersToPoints(double value)
  {
    return (float)(value / 2.54 * 72);
  }

  public static int FontSizeToHps(float value)
  {
    return (int)(value * 2);
  }


  public static int PointsToTwips(float value)
  {
    return (int)(value * 20);
  }


  //public static float CalculateAverageCharacterWidth(string fontName, float fontSize)
  //{
  //  // Create a bitmap to provide a graphics context
  //  using (var bmp = new Bitmap(1, 1))
  //  using (var graphics = Graphics.FromImage(bmp))
  //  {
  //    // Create the font
  //    using (var font = new Font(fontName, fontSize))
  //    {
  //      // Use a sample string with a mix of characters
  //      string sampleText = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

  //      // Measure the string width in pixels
  //      SizeF size = graphics.MeasureString(sampleText, font);

  //      // Calculate the average character width
  //      float averageCharacterWidth = size.Width / sampleText.Length;

  //      return averageCharacterWidth;
  //    }
  //  }
  //}
}