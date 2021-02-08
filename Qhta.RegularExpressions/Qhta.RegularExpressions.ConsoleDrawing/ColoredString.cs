using System;

namespace Qhta.RegularExpressions.ConsoleDrawing
{
  public struct ColoredString
  {
    public String Value;
    public int Color;

    public ColoredString(String value, int color)
    {
      Value = value;
      Color = color;
    }
  }
}
