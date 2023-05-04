using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qhta.TestHelper;

/// <summary>
/// Abstract class for two files compare. One file is "expected", and one is "output" (received).
/// </summary>
public abstract class AbstractFileComparer
{
  /// <summary>
  /// Options of comparizon (e.g. if letter case is ignored). Set up on comparer constructor,
  /// but content can be changed on run.
  /// </summary>
  public FileCompareOptions Options { get; init; }

  /// <summary>
  /// Writer to receive detailed results of comparison. Set up on comparer constructor.
  /// </summary>
  public ITraceTextWriter? Writer { get; init; }

  /// <summary>
  /// Simple constructor
  /// </summary>
  /// <param name="options">Options (instance of class <see cref="FileCompareOptions"/>)</param>
  /// <param name="writer">Writer to receive detailed results of comparison</param>
  [DebuggerStepThrough]
  public AbstractFileComparer(FileCompareOptions options, ITraceTextWriter? writer = null)
  {
    Options = options;
    Writer = writer;
  }

  /// <summary>
  /// Abstract method of file compare
  /// </summary>
  /// <param name="outFilename">File containing received output</param>
  /// <param name="expFilename">File containing expected output</param>
  /// <returns></returns>
  public abstract bool CompareFiles(string outFilename, string expFilename);

  /// <summary>
  /// Check if a line is empty
  /// </summary>
  /// <param name="line">checked line</param>
  /// <returns>true if line contains no characted or only whitespace characters</returns>
  protected virtual bool IsEmpty(string line)
  {
    foreach (var ch in line)
      if (!(char.IsWhiteSpace(ch)))
        return false;
    return true;
  }

  /// <summary>
  /// Method for two string lines comparison. Compare options are applied.
  /// </summary>
  /// <param name="line1">first line to compare</param>
  /// <param name="line2">second line to compare</param>
  /// <returns>true is lines are equal</returns>
  protected virtual bool AreEqual(string line1, string line2)
  {
    if (Options.TrimLines)
    {
      line1 = line1.Trim();
      line2 = line2.Trim();
    }
    if (Options.CompressSpaces)
    {
      line1 = CompressSpaces(line1);
      line2 = CompressSpaces(line2);
    }
    if (Options.IgnoreCase)
    {
      line1 = line1.ToLower(Options.CompareCulture);
      line2 = line2.ToLower(Options.CompareCulture);
      return line1.Equals(line2);
    }
    else
      return line1.Equals(line2);
  }

  /// <summary>
  /// Method to compress subsequent spaces.
  /// </summary>
  /// <param name="line">Input line</param>
  /// <returns>Excessive spaces deleted</returns>
  protected virtual string CompressSpaces(string line)
  {
    List<char> chars = line.ToCharArray().ToList();
    for (int i=0; i<chars.Count-1; i++)
    {
      if (chars[i] == ' ')
      {
        while (i < chars.Count-1 && chars[i+1] == ' ')
          chars.RemoveAt(i+1);
      }
    }
    return new string(chars.ToArray());
  }


  /// <summary>
  /// Send a sequence of lines to the writer.
  /// Colors in options are applied.
  /// </summary>
  /// <param name="lines">lines to write</param>
  /// <param name="isExpected">
  ///   true if lines belong to "expected" file, false if belong to "output" files, or null if none or both
  /// </param>
  protected virtual void ShowLines(string[] lines, bool? isExpected = null)
  {
    bool colorChanged = false;
    if (isExpected == true)
    {
      if (Options.ExpLinesColor!=null && Writer!=null)
      {
        Writer.ForegroundColor = (ConsoleColor)Options.ExpLinesColor;
        colorChanged = true;
      }
    }
    if (isExpected == false)
    {
      if (Options.OutLinesColor != null && Writer!=null)
      {
        Writer.ForegroundColor = (ConsoleColor)Options.OutLinesColor;
        colorChanged = true;
      }
    }
    foreach (var line in lines)
    {
      if (Writer != null)
        Writer.WriteLine(line);
    }
    if (colorChanged)
      Writer?.ResetColors();
  }

  /// <summary>
  /// Send a single line to the writer.
  /// Colors in options are applied.
  /// </summary>
  /// <param name="line">line to write</param>
  /// <param name="isExp">
  ///   true if lines belong to "expected" file, false if belong to "output" files, or null if none or both
  /// </param>
  protected void ShowLine(string line, bool? isExp = null)
  {
    ShowLines(new[] { line }, isExp);
  }

}