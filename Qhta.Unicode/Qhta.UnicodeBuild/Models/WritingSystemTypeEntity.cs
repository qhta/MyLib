using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public partial class WritingSystemTypeEntity
{
  [Key]
  public WritingSystemType Id { get; set; }

  public string Type { get; set; } = null!;

  public string? Description { get; set; }

}