using System.Diagnostics;
using Qhta.TextUtils;

namespace Qhta.Unicode;

/// <summary>
/// Defined a named block used to parse ucd data
/// </summary>
public class NamedBlock
{

  /// <summary>
  /// String representation of the Start and End code points in the block
  /// </summary>
  public string Range
  {
    get => Start.ToString() + (!End.Equals(Start) ? ".." + End.ToString() : "");
    set
    {
      var parts = value.Split("..");
      Start = new CodePoint(parts[0]);
      End = parts.Length > 1 ? new CodePoint(parts[1]) : Start;
    }
  }

  /// <summary>
  /// First code point in the block.
  /// </summary>
  public CodePoint Start { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Last code point in the block. Can be the same as Start
  /// </summary>
  public CodePoint End { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Name of the block.
  /// </summary>
  public string Name { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = null!;

  /// <summary>
  /// A key string of the CodePoint description used to identify the block
  /// </summary>
  public string? KeyString { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// The type of block.
  /// </summary>
  public BlockType BlockType { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Check if a code point is in the block
  /// </summary>
  /// <param name="cp"></param>
  /// <returns></returns>
  public bool Contains(CodePoint cp) => cp >= Start && cp <= End;

  /// <summary>
  /// Check if a code point is in the block
  /// </summary>
  /// <param name="cp"></param>
  /// <param name="description"></param>
  /// <returns></returns>
  public bool Contains(CodePoint cp, string description)
  {
    bool ok = Contains(cp);
    if (!ok) return false;
    if (KeyString is not null)
    {
      return description.IsLike("*"+KeyString+"*");
    }
    return true;
  }
}