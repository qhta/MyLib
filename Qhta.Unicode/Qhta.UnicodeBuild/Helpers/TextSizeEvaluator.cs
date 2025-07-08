using System.Windows;
using System.Windows.Media;

public static class TextSizeEvaluator
{
  public static double EvaluateTextWidth(string text, double fontSize=double.NaN, FontFamily? fontFamily=null, FontStyle fontStyle=default, FontWeight fontWeight=default, FontStretch fontStretch=default)
  {
    if (double.IsNaN(fontSize))
      fontSize = 12; // Default font size
    if (fontFamily==null)
      fontFamily = new FontFamily("Segoe UI"); // Default font family
    if (fontStyle==default)
      fontStyle = FontStyles.Normal;
    if (fontWeight==default)
      fontWeight = FontWeights.Normal;
    if (fontStretch==default)
      fontStretch = FontStretches.Normal;
    var pixelsPerDip = VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;
    var formattedText = new FormattedText(
      text,
      System.Globalization.CultureInfo.CurrentCulture,
      FlowDirection.LeftToRight,
      new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
      fontSize,
      Brushes.Black, // Dummy brush, not used for measurement  
      pixelsPerDip);

    return formattedText.WidthIncludingTrailingWhitespace;
  }
}