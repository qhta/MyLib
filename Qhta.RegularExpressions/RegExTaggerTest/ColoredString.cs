using System;

namespace RegExTaggerTest
{
  public struct ColoredString
  {
    public String Value;
    public ConsoleColor Color;

    public ColoredString(String value, ConsoleColor color)
    {
      Value = value;
      Color = color;
    }
  }
}
