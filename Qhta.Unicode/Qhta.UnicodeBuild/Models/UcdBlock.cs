﻿using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

/// <summary>
/// Represents a Unicode Character Database (UCD) block, which defines a range of Unicode code points and associated
/// metadata.
/// </summary>
/// <remarks>A UCD block is a contiguous range of Unicode code points that share a common purpose or script. This
/// class provides properties to access the block's range, name, and related information.</remarks>
public partial class UcdBlock
{
  /// <summary>
  /// Identifier for the UCD block, which is an integer number of the Id field.
  /// </summary>
  [Key]
  public int? Id { get; set; }

  /// <summary>
  /// String representing the range of Unicode code points in the block, typically formatted as "XXXX..YYYY".
  /// </summary>
  public string? Range { get; set; }

  /// <summary>
  /// Name of the Unicode block, which provides a descriptive label for the block.
  /// </summary>
  public string? BlockName { get; set; }

  /// <summary>
  /// Integer representing the starting code point of the block, which is typically the first code point in the range.
  /// </summary>
  public int? Start { get; set; }

  /// <summary>
  /// Integer representing the ending code point of the block, which is typically the last code point in the range.
  /// </summary>
  public int? End { get; set; }

  /// <summary>
  /// Description or comment about the Unicode block, providing additional context or information about its purpose.
  /// </summary>
  public string? Comment { get; set; }

  /// <summary>
  /// Identifier for the main writing system associated with this UCD block, if applicable.
  /// </summary>
  public int? WritingSystemId { get; set; } // Foreign key property

  /// <summary>
  /// Entity representing the main writing system associated with this UCD block.
  /// </summary>
  public WritingSystem? WritingSystem { get; set; } // Navigation property

}
