using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Qhta.TestHelper
{

  public class TxtFileComparer : FileComparer
  {
    public TxtFileComparer(FileCompareOptions options, ITraceWriter listener) : base(options, listener)
    {
    }

    public override bool CompareFiles(string outFilename, string expFilename)
    {
      string outText;
      string expText;

      using (TextReader aReader = File.OpenText(outFilename))
        outText = aReader.ReadToEnd();

      using (TextReader aReader = File.OpenText(expFilename))
        expText = aReader.ReadToEnd();

      return CompareTexts(outText, expText);
    }

    public bool CompareTexts(string outText, string expText)
    {
      var outLines = outText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      var expLines = expText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      int outIndex, expIndex;
      bool areEqual = true;
      int diffCount = 0;

      bool stopped = false;
      for (outIndex = 0, expIndex = 0; (outIndex >= 0 && outIndex < outLines.Count()) && (expIndex >= 0 && expIndex < expLines.Count()); outIndex++, expIndex++)
      {
        var outLine = outLines[outIndex];
        var expLine = expLines[expIndex];
        if (AreEqual(outLine, expLine))
        {
          if (Options.WriteContentIfEquals)
          {
            ShowLine(outLine);
          }
        }
        else
        {
          areEqual = false;
          if (Options.SyncLimit == 0)
          {
            ShowLine(outLine, false);
            ShowLine(expLine, true);
          }
          else
          {
            if (!TrySynchronize(outLines, ref outIndex, expLines, ref expIndex, Options.SyncLimit))
            {
              stopped = true;
              break;
            }
          }
          diffCount++;
          if (diffCount >= Options.DiffLimit)
          {
            stopped = true;
            break;
          }
        }
      }

      if (areEqual)
      {
        var msg = Options?.EqualityMsg;
        if (msg != null)
          ShowLine(msg);
      }
      else
      {
        var msg = Options?.InequalityMsg;
        if (msg != null)
          ShowLine(msg);
      }
      return areEqual && !stopped;
    }

    protected bool TrySynchronize(string[] outLines, ref int outIndex, string[] expLines, ref int expIndex, int syncLimit)
    {
      var sync = TrySynchronize(outLines, outIndex, expLines, expIndex, syncLimit, out int newOutIndex, out int newExpIndex);
      {
        if (newOutIndex >= outIndex)
        {
          ShowLine(Options.StartOfDiffOut);
          Listener.ForegroundColor = ConsoleColor.Red;
          int count = newOutIndex - outIndex;
          if (count == 0)
            count = 1;
          else if (newExpIndex == expIndex && newOutIndex < outLines.Count() - 1)
            count++;
          ShowLines(outLines.AsSpan(outIndex, count).ToArray(), false);
          Listener.ResetColor();
        }
        outIndex = newOutIndex - 1;
        if (newExpIndex >= expIndex)
        {
          ShowLine(Options.StartOfDiffExp);
          Listener.ForegroundColor = ConsoleColor.Green;
          int count = newExpIndex - expIndex;
          if (count == 0)
            count = 1;
          else if (newOutIndex == outIndex && newExpIndex < expLines.Count() - 1)
            count++;
          ShowLines(expLines.AsSpan(expIndex, count).ToArray(), true);
          Listener.ResetColor();
        }
        expIndex = newExpIndex - 1;
        ShowLine(Options.EndOfDiffs);
      }
      return sync;
    }

    protected bool TrySynchronize(string[] outLines, int outIndex, string[] expLines, int expIndex, int maxDist, out int newOutIndex, out int newExpIndex)
    {
      newOutIndex = outIndex;
      newExpIndex = expIndex;
      var found1 = TrySynchronizeLines(outLines, outIndex, expLines, expIndex, maxDist, out var foundOutIndex1, out var foundExpIndex1);
      var found2 = TrySynchronizeLines(expLines, expIndex, outLines, outIndex, maxDist, out var foundExpIndex2, out var foundOutIndex2);
      if (!found1 && !found2)
        return false;
      newOutIndex = Math.Min(foundOutIndex1, foundOutIndex2);
      newExpIndex = Math.Min(foundExpIndex1, foundExpIndex2);
      return true;
    }

    protected bool TrySynchronizeLines(string[] lines1, int index1, string[] lines2, int index2, int maxDist, out int foundIndex1, out int foundIndex2)
    {
      bool found = false;
      foundIndex1 = int.MaxValue;
      foundIndex2 = int.MaxValue;
      for (int i = index2; i < lines2.Count() && index2 - i <= maxDist; i++)
      {

        if (TryFindEqualLine(lines1, index1, lines2[i], maxDist, out foundIndex1))
        {
          foundIndex2 = i;
          found = true;
          break;
        }
      }
      return found;
    }
    protected bool TryFindEqualLine(string[] lines, int startIndex, string expLine, int maxDist, out int foundIndex)
    {
      for (foundIndex = startIndex; foundIndex < lines.Count() && foundIndex - startIndex <= maxDist; foundIndex++)
      {
        var outLine = lines[foundIndex];
        if (AreEqual(outLine, expLine))
        {
          return true;
        }
      }
      return false;
    }

  }
}
