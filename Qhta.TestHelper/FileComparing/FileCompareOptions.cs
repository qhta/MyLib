using System;
using System.Globalization;

namespace Qhta.TestHelper;

/// <summary>
/// Represents comparing options
/// </summary>
public class FileCompareOptions
{
  /// <summary>
  /// If lines that are empty (without printable characters) are ignored in comparison
  /// </summary>
  public bool IgnoreEmptyLines { get; set; } = false;


  /// <summary>
  /// If spaces starting and ending lines are trimmed before comparison
  /// </summary>
  public bool TrimLines { get; set; } = false;

  /// <summary>
  /// If subsequent spaces are treated as single space
  /// </summary>
  public bool CompressSpaces { get; set; } = false;

  /// <summary>
  /// If letter case is ignored
  /// </summary>
  public bool IgnoreCase { get; set; } = false;

  /// <summary>
  /// Culture to apply if ignore case
  /// </summary>
  public CultureInfo CompareCulture { get; set; } = CultureInfo.InvariantCulture;

  /// <summary>
  /// If attributes order in Xml compare is ignored
  /// </summary>
  public bool IgnoreAttributesOrder { get; set; } = false;

  /// <summary>
  /// if equal lines are written in text compare
  /// </summary>
  public bool WriteContentIfEquals { get; set; }

  /// <summary>
  /// How many different regions are synchronized before return false.
  /// </summary>
  public int DiffLimit { get; set; }

  /// <summary>
  /// What is the maximum distance to search for the same line when difference.
  /// </summary>
  public int SyncLimit { get; set; }

  ///// <summary>
  ///// What is the maximum number of same lines between two diffs to consider as a single diff.
  ///// </summary>
  //public int DiffGapLimit { get; set; }

  /// <summary>
  /// Message written if files are equal
  /// </summary>
  public string? EqualityMsg { get; set; }

  /// <summary>
  /// Message written if files are not equal
  /// </summary>
  public string? InequalityMsg { get; set; }

  /// <summary>
  /// Line written at start of output file different lines or elements
  /// </summary>
  public string StartOfDiffOut { get; set; } = "--- out " + new string('-', 20);

  /// <summary>
  /// Line written at start of expected file different lines or elements
  /// </summary>
  public string StartOfDiffExp { get; set; } = "--- exp " + new string('-', 20);

  /// <summary>
  /// Line written at end of different lines or elements
  /// </summary>
  public string EndOfDiffs { get; set; } = new string('=', 28);

  /// <summary>
  /// Color of output lines if shown in console.
  /// </summary>
  public ConsoleColor? OutLinesColor = ConsoleColor.Red;

  /// <summary>
  /// Color of expected lines if shown in console.
  /// </summary>
  public ConsoleColor? ExpLinesColor = ConsoleColor.Green;

}