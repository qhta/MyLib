using System;

namespace RegExTaggerTest
{
  public struct ColoredChar
  {
    public Char Value;
    public int Color;

    public ColoredChar(Char value, int colorIndex)
    {
      Value = value;
      Color = colorIndex;
    }
  }
}
