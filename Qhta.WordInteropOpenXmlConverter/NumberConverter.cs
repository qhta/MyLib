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
}