using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public partial class WritingSystemKindEntity
{
  [Key]
  public WritingSystemKind Id { get; set; }

  public string Kind { get; set; } = null!;

  public string? Description { get; set; }

}