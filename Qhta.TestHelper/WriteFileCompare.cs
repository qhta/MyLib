using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Qhta.TestHelper
{
  public class WriteCompareOptions
  {
    public bool WriteToConsole { get; set; } = true;

    public bool WriteToDebug { get; set; } = true;

    public bool WriteContentIfEquals { get; set; }

    /// <summary>
    /// How many differences to show before return.
    /// </summary>
    public int DiffLimit { get; set; }


    /// <summary>
    /// What is the maximum distance to search for the same line when difference.
    /// </summary>
    public int SyncLimit { get; set; }

    /// <summary>
    /// What is the maximum number of same lines between two diffs to consider as a single diff.
    /// </summary>
    public int DiffGapLimit { get; set; }

    public string EqualityMsg { get; set; }

    public string InequalityMsg { get; set; }
  }

  public static class WriteFileCompare
  {
    public static bool CompareFiles(string outFilename, string expFilename, WriteCompareOptions options)
    {
      string outText;
      string expText;

      using (TextReader aReader = File.OpenText(outFilename))
        outText = aReader.ReadToEnd();

      using (TextReader aReader = File.OpenText(expFilename))
        expText = aReader.ReadToEnd();

      return CompareTexts(outText, expText, options);
    }

    public static bool CompareTexts(string outText, string expText, WriteCompareOptions options)
    {
      var outLines = outText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      var expLines = expText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      int outIndex, expIndex;
      bool areEqual = true;
      int diffCount = 0;

      int syncLimit = 0;
      if (options != null)
        syncLimit = options.SyncLimit;

      int diffLimit = 0;
      if (options != null)
        diffLimit = options.DiffLimit;

      bool stopped = false;
      bool writeEqualLine = options != null && options.WriteContentIfEquals;
      bool writeToConsole = options != null || options.WriteToConsole;
      bool writeToDebug = options != null || options.WriteToDebug;
      for (outIndex = 0, expIndex = 0; outIndex < outLines.Count() && expIndex < expLines.Count(); outIndex++, expIndex++)
      {
        var outLine = outLines[outIndex];
        var expLine = expLines[expIndex];
        if (outLine.Trim() == expLine.Trim())
        {
          if (writeEqualLine)
          {
            if (writeToConsole)
              Console.WriteLine(outLine);
            if (writeToDebug)
              Debug.WriteLine("    " + outLine);
          }
        }
        else
        {
          areEqual = false;
          if (syncLimit == 0)
          {
            if (writeToConsole)
            {
              var color = Console.ForegroundColor;
              Console.ForegroundColor = ConsoleColor.Red;
              Console.WriteLine(outLine);
              Console.ForegroundColor = ConsoleColor.Green;
              Console.WriteLine(expLine);
              Console.ForegroundColor = color;
            }
            if (writeToDebug)
            {
              Debug.WriteLine("out1:" + outLine);
              Debug.WriteLine("exp1:" + expLine);
            }
          }
          else
          {
            if (!TrySynchronize(outLines, ref outIndex, expLines, ref expIndex, syncLimit, options))
              break;
          }
          diffCount++;
          if (diffCount >= diffLimit)
          {
            stopped = true;
            break;
          }
        }
      }
      if (!stopped)
      {
        if (outIndex < outLines.Count())
        {
          var color = Console.ForegroundColor;
          Console.ForegroundColor = ConsoleColor.Red;
          if (writeToConsole)
            Console.WriteLine(new string('-', 20));
          if (writeToDebug)
            Debug.WriteLine(new string('-', 20));
          for (; outIndex < outLines.Count(); outIndex++)
          {
            if (writeToConsole)
              Console.WriteLine(outLines[outIndex]);
            if (writeToDebug)
              Debug.WriteLine("out:" + outLines[outIndex]);
          }
          Console.ForegroundColor = color;
        }
        if (expIndex < expLines.Count())
        {
          var color = Console.ForegroundColor;
          Console.ForegroundColor = ConsoleColor.Green;
          if (writeToConsole)
            Console.WriteLine(new string('-', 20));
          if (writeToDebug)
            Debug.WriteLine(new string('-', 20));
          for (; expIndex < expLines.Count(); expIndex++)
          {
            if (writeToConsole)
              Console.WriteLine(expLines[expIndex]);
            if (writeToDebug)
              Debug.WriteLine("exp:" + expLines[expIndex]);
          }
          Console.ForegroundColor = color;
        }
      };

      if (areEqual)
      {
        var msg = options?.EqualityMsg;
        if (msg != null)
        {
          if (writeToConsole)
            Console.WriteLine(msg);
          if (writeToDebug)
            Debug.WriteLine(msg);
        }
      }
      else
      {
        var msg = options?.InequalityMsg;
        {
          if (writeToConsole)
            Console.WriteLine(msg);
          if (writeToDebug)
            Debug.WriteLine(msg);
        }
      }
      return areEqual;
    }

    private static string EndOfDiffs = new string('-', 20);

    private static bool TrySynchronize(string[] outLines, ref int outIndex, string[] expLines, ref int expIndex, int syncLimit, WriteCompareOptions options)
    {
      int gapLimit = 0;
      if (options != null)
        gapLimit = options.DiffGapLimit;
      if (Synchronize(outLines, outIndex, expLines, expIndex, syncLimit, gapLimit, out int newOutIndex, out int newExpIndex))
      {
        bool writeToConsole = options != null || options.WriteToConsole;
        bool writeToDebug = options != null || options.WriteToDebug;
        string prefix = "   ";
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        if (newOutIndex >= outIndex)
        {
          prefix = "out:";
          do
          {
            if (writeToConsole)
              Console.WriteLine(outLines[outIndex]);
            if (writeToDebug)
              Debug.WriteLine(prefix + outLines[outIndex]);
            prefix = "    ";
            if (outIndex >= newOutIndex - 1)
              break;
            outIndex++;
          } while (true);
        }
        outIndex = newOutIndex - 1;
        Console.ForegroundColor = ConsoleColor.Green;
        if (newExpIndex >= expIndex)
        {
          prefix = "exp:";
          do
          {
            if (writeToConsole)
              Console.WriteLine(expLines[expIndex]);
            if (writeToDebug)
              Debug.WriteLine(prefix + expLines[expIndex]);
            prefix = "    ";
            if (expIndex >= newExpIndex - 1)
              break;
            expIndex++;
          } while (true);
        }
        expIndex = newExpIndex - 1;
        Console.ForegroundColor = color;
        if (writeToConsole)
          Console.WriteLine(EndOfDiffs);
        if (writeToDebug)
          Debug.WriteLine(EndOfDiffs);

        return true;
      }
      return false;
    }

    private static bool Synchronize(string[] outLines, int outIndex, string[] expLines, int expIndex, int maxDist, int maxGap, out int newOutIndex, out int newExpIndex)
    {
      bool gapFound = false;
      bool found = false;
      do
      {
        int minOutIndex = int.MaxValue;
        for (int i = expIndex; i < expLines.Count() && expIndex - i <= maxDist; i++)
        {
          if (TryFindEqualLine(outLines, outIndex, expLines[i], maxDist, out var foundOutIndex))
            if (foundOutIndex < minOutIndex)
              minOutIndex = foundOutIndex;
        }
        int minExpIndex = int.MaxValue;
        for (int i = outIndex; i < outLines.Count() && expIndex - i <= maxDist; i++)
        {
          if (TryFindEqualLine(expLines, expIndex, outLines[i], maxDist, out var foundExpIndex))
            if (foundExpIndex < minExpIndex)
              minExpIndex = foundExpIndex;
        }
        if (minOutIndex < int.MaxValue)
        {
          found = true;
          newOutIndex = minOutIndex;
        }
        else
          newOutIndex = -1;
        if (minExpIndex < int.MaxValue)
        {
          found = true;
          newExpIndex = minExpIndex;
        }
        else
          newExpIndex = -1;
        gapFound = false;
        if (found)
        {
          outIndex = newOutIndex;
          expIndex = newExpIndex;
          for (int i = 2; i <= maxGap; i++)
          {
            outIndex++;
            expIndex++;
            if (outIndex < outLines.Count() && expIndex < expLines.Count())
            {
              var outLine = outLines[outIndex];
              var expLine = expLines[expIndex];
              if (outLine.Trim() != expLine.Trim())
              {
                //outIndex--;
                //expIndex--;
                gapFound = true;
                break;
              }
            }
          }
          if (gapFound)
          {
            newOutIndex = outIndex;
            newExpIndex = expIndex;
          }
        }
      } while (gapFound);
      return found;
    }

    private static bool TryFindEqualLine(string[] lines, int startIndex, string expLine, int maxDist, out int foundIndex)
    {
      for (foundIndex = startIndex; foundIndex < lines.Count() && foundIndex - startIndex <= maxDist; foundIndex++)
        if (lines[foundIndex] == expLine)
        {
          return true;
        }
      return false;
    }

  }
}
