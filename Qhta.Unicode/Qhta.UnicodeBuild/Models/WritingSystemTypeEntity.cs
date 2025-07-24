using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Qhta.Unicode.Models;

/// <summary>
/// Entity representing a writing system type.
/// </summary>
public partial class WritingSystemTypeEntity
{
  /// <summary>
  /// Identifier for the writing system type, represented as a <see cref="WritingSystemType"/> enum.
  /// </summary>
  [Key]
  public WritingSystemType Id { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Name of the writing system type.
  /// </summary>
  [Column("Type")]
  public string Name { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// Description of the writing system type.
  /// </summary>
  public string? Description { [DebuggerStepThrough] get; set; }

}