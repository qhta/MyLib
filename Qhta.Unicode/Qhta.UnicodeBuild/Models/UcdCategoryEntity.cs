using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Qhta.Unicode.Models;

/// <summary>
/// Entity representing a Unicode category.
/// </summary>
[Table("Categories")]
public partial class UcdCategoryEntity
{
  /// <summary>
  /// Identifier for the Unicode category, which is an enumeration value of type UcdCategory.
  /// </summary>
  [Key]
  public UcdCategory Id { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Two-letter code representing the Unicode category, typically used for classification purposes.
  /// </summary>
  public string Ctg { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// Long name of the Unicode category, providing a more descriptive label for the category.
  /// </summary>
  public string? Name { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Description of the Unicode category, explaining its purpose or characteristics.
  /// </summary>
  public string? Comment { [DebuggerStepThrough] get; set; }


}