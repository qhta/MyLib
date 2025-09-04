using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Qhta.Unicode.Models;

/// <summary>
/// An alias entity representing a Unicode code point alias.
/// </summary>
public partial class Alias
{
  /// <summary>
  /// Prime key identifier for the alias entity.
  /// </summary>
  [Key]
  public int Id { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Ordinal number of the alias associated with the specific code point.
  /// </summary>
  public int Ord { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Alias name, which is a short identifier for the alias.
  /// </summary>
  public string Name { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// Type of the alias, which is represented as a byte value.
  /// </summary>
  public AliasType Type { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Code point entity that this alias refers to, represented by a foreign key relationship.
  /// </summary>
  public virtual UcdCodePoint UcdCodePoint { [DebuggerStepThrough] get; set; } = null!;
}
