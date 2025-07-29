using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Qhta.Unicode.Models;

/// <summary>
/// Represents an entity that defines a type of writing system.
/// </summary>
/// <remarks>This class is used to categorize different writing systems by their kind and description.</remarks>
public partial class WritingSystemKindEntity
{
  /// <summary>
  /// Identifier for the writing system kind, represented as a <see cref="WritingSystemKind"/> enum.
  /// </summary>
  [Key]
  public WritingSystemKind Id { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Name of the writing system kind.
  /// </summary>
  [Column("Name")]
  public string Name { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// Type of the writing system to which this kind applies.
  /// </summary>
  [Column("Type")]
  public WritingSystemType? Type { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// Description of the writing system kind.
  /// </summary>
  public string? Description { [DebuggerStepThrough] get; set; }

}