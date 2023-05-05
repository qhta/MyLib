using System;
using System.IO;
using System.Linq;

namespace Qhta.TestHelper;

/// <summary>
/// Concrete comparer for text files
/// </summary>
public class TextFileComparer : AbstractFileComparer
{
  /// <summary>
  /// Constructor invoking base class constructor
  /// </summary>
  /// <param name="options">Options to compare</param>
  /// <param name="writer">Writer used to show different lines</param>
  public TextFileComparer(FileCompareOptions options, ITraceTextWriter writer) : base(options, writer)
  {
  }

  /// <summary>
  /// Implemented method of file comparison.
  /// Simply reads out whole files and invokes <see cref="CompareTexts"/>.
  /// </summary>
  /// <param name="recFilename">Received content filename</param>
  /// <param name="expFilename">Expected content filename</param>
  /// <returns>true is both files are equal</returns>
  public override bool CompareFiles(string recFilename, string expFilename)
  {
    string recText;
    string expText;

    using (TextReader aReader = File.OpenText(recFilename))
      recText = aReader.ReadToEnd();

    using (TextReader aReader = File.OpenText(expFilename))
      expText = aReader.ReadToEnd();

    return CompareTexts(recText, expText);
  }

  /// <summary>
  /// Main text comparison method. 
  /// Input texts are splitted to lines using '\n' character and "\r\n" character pairs.
  /// Then <see cref="CompareLines"/> is called.
  /// </summary>
  /// <param name="recText">Received content text</param>
  /// <param name="expText">Expected content text</param>
  /// <returns>true if both texts are equal</returns>
  protected virtual bool CompareTexts(string recText, string expText)
  {
    recText = recText.Replace("\r\n", "\n");
    expText = expText.Replace("\r\n", "\n");
    var recLines = recText.Split('\n');
    var expLines = expText.Split('\n');
    return CompareLines(recLines, expLines);
  }

  /// <summary>
  /// Main method to compare collection of lines.
  /// Subsequent lines in the first and second collections are compared.
  /// If they are not equal then comparer tries to synchronize both collections,
  /// i.e. to find next equal lines. Unequal lines are shown using writer
  /// </summary>
  /// <param name="recLines">Received content text lines</param>
  /// <param name="expLines">Expected content text lines</param>
  /// <returns>true if both lines collections are equal</returns>
  protected virtual bool CompareLines(string[] recLines, string[] expLines)
  {
    int outIndex, expIndex;
    bool areEqual = true;
    int diffCount = 0;

    bool stopped = false;
    for (outIndex = 0, expIndex = 0; (outIndex >= 0 && outIndex < recLines.Count()) && (expIndex >= 0 && expIndex < expLines.Count()); outIndex++, expIndex++)
    {
      var outLine = recLines[outIndex];
      var expLine = expLines[expIndex];
      if (Options.IgnoreEmptyLines)
      {
        if (IsEmpty(outLine))
        {
          outIndex--;
          continue;
        }
        if (IsEmpty(expLine))
        {
          expIndex--;
          continue;
        }
      }
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
        diffCount++;
        if (diffCount > Options.DiffLimit)
        {
          stopped = true;
          break;
        }
        if (Options.SyncLimit == 0)
        {
          ShowLine(outLine, Options.RecLinesColor);
          ShowLine(expLine, Options.ExpLinesColor);
        }
        else
        {
          if (!TrySynchronize(recLines, outIndex, expLines, expIndex, Options.SyncLimit, out int newOutIndex, out int newExpIndex))
          {
            stopped = true;
            break;
          }
          else
          { // -1 is needed to avoid skipping equal lines in next iteration
            outIndex = newOutIndex - 1;
            expIndex = newExpIndex - 1;
          }
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

  /// <summary>
  /// A method to synchronize output lines and expected lines if the difference is found.
  /// First different line in <paramref name="recLines"/> is pointed by <paramref name="outIndex"/>
  /// and first different line in <paramref name="expLines"/> is pointed by <paramref name="outIndex"/>
  /// Subsequent lines are compared in a maximum distance of <paramref name="maxDist"/>.
  /// New indexes <paramref name="newOutIndex"/> and <paramref name="newExpIndex"/> point the fist equal lines.
  /// If synchronization was successful then the different lines are shown.
  /// </summary>
  /// <param name="recLines">Received content text lines</param>
  /// <param name="outIndex">Index of first different line in <paramref name="recLines"/> collection</param>
  /// <param name="expLines">Expected content text lines</param>
  /// <param name="expIndex">Index of first different line in <paramref name="expLines"/> collection</param>
  /// <param name="maxDist">Maximum distance of search for the equal lines</param>
  /// <param name="newOutIndex">Index of the first equal line in <paramref name="recLines"/> collection</param>
  /// <param name="newExpIndex">Index of the first equal line in <paramref name="expLines"/> collection</param>
  /// <returns>true if equal lines found</returns>
  protected bool TrySynchronize(string[] recLines, int outIndex, string[] expLines, int expIndex, int maxDist, out int newOutIndex, out int newExpIndex)
  {
    var sync = TrySync(recLines, outIndex, expLines, expIndex, maxDist, out newOutIndex, out newExpIndex);
    if (sync)
    {
      if (newOutIndex > outIndex)
      {
        ShowLine(Options.StartOfDiffRec);
        int count = newOutIndex - outIndex;
        ShowLines(recLines.AsSpan(outIndex, count).ToArray(), Options.RecLinesColor);
      }
      if (newExpIndex > expIndex)
      {
        ShowLine(Options.StartOfDiffExp);
        int count = newExpIndex - expIndex;
        ShowLines(expLines.AsSpan(expIndex, count).ToArray(), Options.ExpLinesColor);
      }
      ShowLine(Options.EndOfDiffs);
    }
    return sync;
  }

  /// <summary>
  /// Synchronization helper method.
  /// First tries to find equal line in first collection taking subsequent lines from the second one,
  /// next tries to find equal line in second collection taking subsequent lines from the first one.
  /// If not found in first and not in second trial then synchronization failes.
  /// Otherwise counts the distance of the first and the second trial and returns new indexes of the shorten one.
  /// </summary>
  /// <param name="lines1">First collection to search</param>
  /// <param name="index1">Index of search start in the first collection</param>
  /// <param name="lines2">Second collection to search</param>
  /// <param name="index2">Index of search start in the second collection</param>
  /// <param name="maxDist">Maximum distance of search</param>
  /// <param name="newIndex1">Index of found equal line in first collection, maxint if not found</param>
  /// <param name="newIndex2">Index of found equal line in second collection, maxint if not found</param>
  /// <returns>true if equal lines found</returns>
  protected bool TrySync(string[] lines1, int index1, string[] lines2, int index2, int maxDist, out int newIndex1, out int newIndex2)
  {
    newIndex1 = index1;
    newIndex2 = index2;
    var trial1Found = TryFindEqualLines(lines1, index1, lines2, index2, maxDist, out var trial1Index1, out var trial1Index2);
    var trial2Found = TryFindEqualLines(lines2, index2, lines1, index1, maxDist, out var trial2Index1, out var trial2Index2);
    if (!trial1Found && !trial2Found)
      return false;
    var trial1Distance = (trial1Index1 - index1) + (trial1Index2 - index2);
    var trial2Distance = (trial2Index1 - index1) + (trial2Index2 - index2);
    if (trial1Distance < trial2Distance)
    {
      newIndex1 = trial1Index1;
      newIndex2 = trial1Index2;
    }
    else
    {
      newIndex1 = trial2Index1;
      newIndex2 = trial2Index2;
    }
    return true;
  }

  /// <summary>
  /// Synchronization helper method.
  /// Takes subsequent lines from the second collection and searches for the equal line in the first collection.
  /// If the equal lines are found, both <paramref name="newIndex1"/> and <paramref name="newIndex2"/> 
  /// point out equal lines.
  /// The search is limited to <paramref name="maxDist"/> distance.
  /// If option <see cref="FileCompareOptions.IgnoreEmptyLines"/> is set 
  /// then empty lines are ignored but are included in counting distance.
  /// </summary>
  /// <param name="lines1">First collection to search</param>
  /// <param name="index1">Index of search start in the first collection</param>
  /// <param name="lines2">Second collection to search</param>
  /// <param name="index2">Index of search start in the second collection</param>
  /// <param name="maxDist">Maximum distance of search</param>
  /// <param name="newIndex1">Index of found equal line in first collection, maxint if not found</param>
  /// <param name="newIndex2">Index of found equal line in second collection, maxint if not found</param>
  /// <returns>true if equal lines found</returns>
  protected bool TryFindEqualLines(string[] lines1, int index1, string[] lines2, int index2, int maxDist, out int newIndex1, out int newIndex2)
  {
    bool found = false;
    newIndex1 = int.MaxValue;
    newIndex2 = int.MaxValue;
    for (int i = index2; i < lines2.Count() && index2 - i <= maxDist; i++)
    {
      if (Options.IgnoreEmptyLines && IsEmpty(lines2[i]))
        continue;

      if (TryFindEqualLine(lines1, index1, lines2[i], maxDist, out newIndex1))
      {
        newIndex2 = i;
        found = true;
        break;
      }
    }
    return found;
  }

  /// <summary>
  /// Synchronization helper method.
  /// Searches in a <paramref name="lines"/> collection starting at <paramref name="startIndex"/>
  /// for a line that is equal to <paramref name="expLine"/>.
  /// The search is limited to <paramref name="maxDist"/> distance.
  /// After search the fist equal line is pointed by <paramref name="foundIndex"/> parameter.
  /// If option <see cref="FileCompareOptions.IgnoreEmptyLines"/> is set 
  /// then empty lines are ignored but are included in counting distance.
  /// </summary>
  /// <param name="lines">Collection of lines to search</param>
  /// <param name="startIndex">Start search index</param>
  /// <param name="expLine">Expected line</param>
  /// <param name="maxDist">Maximum distance of search</param>
  /// <param name="foundIndex"></param>
  /// <returns>True if equal line was found. If not, the <paramref name="foundIndex"/> should be ignored</returns>
  protected bool TryFindEqualLine(string[] lines, int startIndex, string expLine, int maxDist, out int foundIndex)
  {
    for (foundIndex = startIndex; foundIndex < lines.Count() && foundIndex - startIndex <= maxDist; foundIndex++)
    {
      var outLine = lines[foundIndex];

      if (Options.IgnoreEmptyLines && IsEmpty(outLine))
        continue;

      if (AreEqual(outLine, expLine))
      {
        return true;
      }
    }
    return false;
  }

}