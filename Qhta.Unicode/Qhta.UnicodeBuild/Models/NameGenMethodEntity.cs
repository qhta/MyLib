using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Qhta.Unicode.Models;

/// <summary>
/// Entity representing a name generation method.
/// </summary>
public partial class NameGenMethodEntity
{
  /// <summary>
  /// Identifier for the name generation method, represented as a <see cref="NameGenMethod"/> enum.
  /// </summary>
  [Key]
  public NameGenMethod Id { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Name of the name generation method.
  /// </summary>
  [Column("Name")]
  public string Name { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// Description of the name generation method.
  /// </summary>
  public string? Description { [DebuggerStepThrough] get; set; }

}