using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Qhta.RegularExpressions.ConsoleDrawing
{
  public class RegExConsolePresenter
  {
    static List<ConsoleColor> ConsoleColors = new List<ConsoleColor>
    {
      ConsoleColor.Green,
      ConsoleColor.Red,
      ConsoleColor.Cyan,
      ConsoleColor.Magenta,
      ConsoleColor.Yellow,
    };

    public void DrawGraph(TextWriter writer, string pattern, RegExItems items)
    {
      GraphItems graphItems = new GraphItems(items);
      List<GraphItem> itemsList = graphItems.ToList();
      SetupColors(itemsList);
      var plainText = graphItems.PlainText;
      var xCount = plainText.Length + MaxItemDepth(graphItems);
      var yCount = itemsList.Count;
      ColoredChar[,] frameMatrix = new ColoredChar[xCount, yCount];
      for (int x = 0; x < xCount; x++)
        for (int y = 0; y < yCount; y++)
        {
          frameMatrix[x, y] = new ColoredChar(' ', 0xFFFFFF);
        }

      for (int i = itemsList.Count - 1; i >= 0; i--)
      {
        var item = itemsList[i];

        int yMax = yCount - i - 1;
        item.YMax = yMax;
        int color = item.Color;

        if (item.SubItems == null && yMax != 0)
        {
          if (item.Length == 1)
          {
            for (int y = 0; y < yMax; y++)
            {
              frameMatrix[item.ColStart, y] = new ColoredChar('│', color); // vertical line for 1-char item
            }
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
          }
          else
          {
            frameMatrix[item.ColStart, 0] = new ColoredChar('├', color);
            for (int x = 1; x < item.ColCount - 2; x++)
            {
              frameMatrix[item.ColStart + x, 0] = new ColoredChar('─', color);
            }
            frameMatrix[item.ColStart + item.ColCount - 2, 0] = new ColoredChar('┘', color);
            for (int y = 1; y < yMax; y++)
            {
              frameMatrix[item.ColStart, y] = new ColoredChar('│', color); // left vertical line for simple item when in group
            }
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
          }
        }
        else
        {
          for (int y = 0; y < yMax; y++)
          {
            frameMatrix[item.ColStart, y] = new ColoredChar('│', color); // left vertical line for group item
            if (item.Length > 1)
            {
              frameMatrix[item.ColStart + item.ColCount - 2, y] = new ColoredChar('│', color); // right vertical line for group item
            }
          }

          if (item.Length == 1)
          {
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
          }
          else
          {
            frameMatrix[item.ColStart, yMax] = new ColoredChar('└', color);
            for (int x = 1; x < item.ColCount; x++)
            {
              frameMatrix[item.ColStart + x, yMax] = new ColoredChar('─', color); // horizontal line for group item
            }
            frameMatrix[item.ColStart + item.ColCount - 2, yMax] = new ColoredChar('┴', color);
            ExtendHorizontalLine(item, frameMatrix, item.ColStart + item.ColCount);
            //if (item.SubItems != null)
            //{
            //  if (item.PlainText.StartsWith("$"))
            //    Debug.Assert(true);
            //  for (int subIndex=0; subIndex<item.SubItems.Count; subIndex++)
            //  {
            //    var subItem = item.SubItems[subIndex];
            //    int lineColor = subItem.Color;
            //    for (int x = subItem.ColStart + 1; x<item.ColCount+1; x++)
            //    {
            //      var y = subItem.YMax;
            //      frameMatrix[x, y] = new ColoredChar('─', lineColor);
            //    }

            //  }
              //for (int y = yMax - 1; y >= 0; y--)
              //{
              //int lineColor = -1;
              //var subIndex = yMax-y-1
              //for (int x = item.ColStart + item.ColCount - 3; x >= 0; x--)


              //int lineColor = -1;
              //for (int x = item.ColStart + item.ColCount - 3; x >= 0; x--)
              //{
              //  var curChar = frameMatrix[x, y].Value;
              //  if (curChar == '└' || curChar == '─' || curChar == '┴')
              //  {
              //    lineColor = frameMatrix[x, y].Color;
              //    break;
              //  }
              //  else
              //  if (curChar != ' ')
              //  {
              //    break;
              //  }
              //}
              //if (lineColor != -1)
              //{
              //  for (int x = item.ColStart + item.ColCount - 3; x >= 0; x--)
              //  {
              //    if (frameMatrix[x, y].Value == ' ')
              //    {
              //      frameMatrix[x, y] = new ColoredChar('─', lineColor); // right horizontal extension line for single item
              //    }
              //    else
              //      break;
              //  }
              //  for (int x = item.ColStart + item.ColCount; x >= item.ColStart + item.ColCount - 1; x--)
              //    frameMatrix[x, y] = new ColoredChar('─', lineColor); // right horizontal extension line for items in group item
              //}
              //}
            //}
          }
        }
      }

      if (writer == Console.Out)
      {
        var coloredText = graphItems.ColoredText;
        foreach (var item in coloredText)
        {
          Console.ForegroundColor = GetConsoleColor(item.Color);
          Console.Write(item.Value);
        }
        //if (items.Inequality != null)
        //  Console.Write($"  Items.Count={items.Inequality.Obtained} expected={items.Inequality.Expected}"); 
        Console.WriteLine();
      }
      else
      {
        writer.WriteLine(graphItems.PlainText);
      }
      for (int i = itemsList.Count - 1; i >= 0; i--)
      {
        var item = itemsList[i];
        int yMax = yCount - i - 1;
        int l = xCount;
        while (l > 0 && frameMatrix[l - 1, yMax].Value == ' ') l--;
        if (writer == Console.Out)
        {
          for (int x = 0; x < l; x++)
          {
            Console.ForegroundColor = GetConsoleColor(frameMatrix[x, yMax].Color);
            Console.Write(frameMatrix[x, yMax].Value);
          }
          Console.WriteLine(" " + item.ToString());
          Console.ResetColor();
        }
        else
        {
          string frameLine = "";
          if (l > 0)
          {
            var frameRow = new char[l];
            for (int x = 0; x < l; x++)
              frameRow[x] = frameMatrix[x, yMax].Value;
            frameLine = new string(frameRow);
          }
          writer.Write(frameLine);
          var s = $"{item}";
          writer.WriteLine($" {s}");
        }
      }
    }

    private int MaxItemDepth(GraphItems items)
    {
      var result = 0;
      foreach (var item in items)
      {
        var n = MaxItemDepth(item);
        if (n > result)
          result = n;
      }
      return result;
    }

    private int MaxItemDepth(GraphItem item)
    {
      if (item.SubItems==null)
        return 1;
      var result = 1;
      foreach (var subItem in item.SubItems)
      {
        result += MaxItemDepth(subItem);
      }
      return result;
    }

    private void ExtendHorizontalLine(GraphItem item, ColoredChar[,] frameMatrix, int xMax)
    {
      if (item.SubItems != null)
      {
        for (int subIndex = 0; subIndex < item.SubItems.Count; subIndex++)
        {
          var subItem = item.SubItems[subIndex];
          int lineColor = subItem.Color;
          for (int x = subItem.ColStart + 1; x <= xMax; x++)
          {
            var y = subItem.YMax;
            if (frameMatrix[x, y].Value ==' ')
              frameMatrix[x, y] = new ColoredChar('─', lineColor);
          }
          ExtendHorizontalLine(subItem, frameMatrix, xMax);
        }
      }
    }

    /// <summary>
    /// Setup colors in list of graph items and keep each adjacent colors different
    /// </summary>
    /// <param name="items"></param>
    private void SetupColors(List<GraphItem> items)
    {
      SetupColors(items, 0, -1);
      int priorColor = -1;
      foreach (var item in items)
      {
        if (item.Color == priorColor)
        {
          item.Color = (item.Color + 1) % ConsoleColors.Count;
        }
        priorColor = item.Color;
      }
    }

    /// <summary>
    /// Recurrent method of colors setup. 
    /// When iterate in subitems, it keeps that the last subitem will have different color than closing mark of the group.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="colorIndex"></param>
    /// <param name="groupColor"></param>
    /// <returns></returns>
    static int SetupColors(List<GraphItem> items, int colorIndex, int groupColor)
    {
      var lastColorIndex = -1;
      int itemColor = -1;
      for (int i = 0; i < items.Count; i++)
      {
        var item = items[i];
        int index = item.Start;
        itemColor = colorIndex;
        if (i == items.Count - 1 && itemColor == groupColor)
        {
          colorIndex = (colorIndex + 1) % ConsoleColors.Count;
          itemColor = colorIndex;
        }
        item.Color = itemColor;
        lastColorIndex = colorIndex;
        colorIndex = (colorIndex + 1) % ConsoleColors.Count;
        if (item.SubItems != null)
        {
          var lastColor = SetupColors(item.SubItems, colorIndex, itemColor);
          if (lastColor == colorIndex)
            colorIndex = (colorIndex + 1) % ConsoleColors.Count;
        }
      }
      return lastColorIndex;
    }

    static ConsoleColor GetConsoleColor(int colorIndex)
    {
      if (colorIndex == 0xFFFFFF)
        return ConsoleColor.White;
      return ConsoleColors[colorIndex];
    }


  }
}
