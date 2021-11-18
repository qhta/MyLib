using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Qhta.TestHelper
{
  public class WriteCompareOptions
  {
    public bool WriteToConsole { get; set; } = true;

    public bool WriteToDebug { get; set; } = true;

    public bool WriteContentIfEquals { get; set; }

    public bool StopAfterFirstDiff { get; set; }

    public bool StopAfterManyDiff { get; set; }

    public int DiffLimit { get; set; } = 10;

    public int SynchronizationLimit { get; set; } = 10;

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
              Debug.WriteLine("    "+ outLine);
          }
        }
        else
        {
          areEqual = false;
          if (options != null && options.StopAfterFirstDiff)
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
              Debug.WriteLine("out:" + outLine);
              Debug.WriteLine("exp:" + expLine);
            }
            stopped = true;
            break;
          }
          diffCount++;
          if (options != null && options.StopAfterManyDiff)
          {
            int diffLimit = 10;
            if (options.DiffLimit > 0)
              diffLimit = options.DiffLimit;
            if (diffCount >= diffLimit)
            {
              stopped = true;
              break;
            }
          }
          int syncLimit = 10;
          if (options != null && options.SynchronizationLimit > 0)
            syncLimit = options.SynchronizationLimit;
          if (TrySynchronize(outLines, outIndex, expLines, expIndex, syncLimit, out int newOutIndex, out int newExpIndex))
          {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (newOutIndex > outIndex)
              do
              {
                if (writeToConsole)
                  Console.WriteLine(outLines[outIndex]);
                if (writeToDebug)
                  Debug.WriteLine("out:" +outLines[outIndex]);
                if (outIndex >= newOutIndex - 1)
                  break;
                outIndex++;
              } while (true);
            else
              outIndex--;
            Console.ForegroundColor = ConsoleColor.Green;
            if (newExpIndex > expIndex)
              do
              {
                if (writeToConsole)
                  Console.WriteLine(expLines[expIndex]);
                if (writeToDebug)
                  Debug.WriteLine("exp:" + outLines[outIndex]);
                if (expIndex >= newExpIndex - 1)
                  break;
                expIndex++;
              } while (true);
            else
              expIndex--;
            Console.ForegroundColor = color;
          }
          else
            break;
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
              Debug.WriteLine("out:"+outLines[outIndex]);
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

    private static bool TrySynchronize(string[] outLines, int outIndex, string[] expLines, int expIndex, int maxDist, out int newOutIndex, out int newExpIndex)
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
      bool found = false;
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
