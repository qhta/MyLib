using System;

namespace RegExTaggerTest
{
  public struct ColoredChar
  {
    public Char Value;
    public ConsoleColor Color;

    public ColoredChar(Char value, ConsoleColor color)
    {
      Value = value;
      Color = color;
    }
  }
}
