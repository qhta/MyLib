using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

/// <summary>
/// Mapping between Unicode code point ranges and writing systems.
/// Read from the mapping files.
/// </summary>
public class WritingSystemMapping
{
  /// <summary>
  /// Represents a range of Unicode code points, typically formatted as "XXXX..YYYY".
  /// </summary>
  [Key]
  public string Range { get; set; } = null!;
  /// <summary>
  /// Represents the name for the writing system associated with this mapping.
  /// </summary>
  public string? WritingSystemName { get; set; }
}