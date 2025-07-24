using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Qhta.Unicode.Models;

/// <summary>
/// Entity representing a writing system, which can be a script, language, or notation.
/// </summary>
public partial class WritingSystem
{
  /// <summary>
  /// Number that uniquely identifies the writing system in the database.
  /// </summary>
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int? Id { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Name of the writing system (in English).
  /// </summary>
  public string? Name { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// A type of writing system, such as script, language, or notation.
  /// </summary>
  public WritingSystemType? Type { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A kind of writing system, such as alphabet, syllabary, or logographic.
  /// </summary>
  public WritingSystemKind? Kind { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Identifier of the parent writing system, if this writing system is a child of another.
  /// </summary>
  public int? ParentId { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Key phrase which identifies the writing system in CodePoint Description field.
  /// It may contain a wildcard character ('*') which tells the system
  /// whether the key phrase is placed at the beginning or at the end of the CodePoint Description field.
  /// </summary>
  public string? KeyPhrase { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Unicode Category (Ctg) code, which may be used to classify the writing system using the Ctg field of the CodePoints.
  /// </summary>
  public string? Ctg { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Standardized ISO code name for the writing system, if applicable.
  /// </summary>
  public string? Iso { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// An abbreviation for the writing system used in the target serialization method, if applicable.
  /// </summary>
  public string? Abbr { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Extension string added to the Abbr field to provide unique identification for the writing system,
  /// </summary>
  public string? Ext { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Description of the writing system, which explains its meaning.
  /// </summary>
  public string? Description { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// An optional parent writing system that this writing system is derived from or associated with.
  /// Associated with the ParentId field.
  /// </summary>
  public virtual WritingSystem? ParentSystem { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A collection of child writing systems that are derived from or associated with this writing system.
  /// </summary>
  public virtual ICollection<WritingSystem>? Children { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// A count of the number of child writing systems associated with this writing system.
  /// Zero if there are no children.
  /// </summary>
  public int ChildrenCount => Children?.Count ?? 0;

  /// <summary>
  /// A collection of UcdBlocks that contain code points associated with this writing system.
  /// </summary>
  public virtual ICollection<UcdBlock>? UcdBlocks { [DebuggerStepThrough] get; set; }

}
