using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public class WritingSystemMapping
{
  [Key]
  public string Range { get; set; } = null!;
  public string? WritingSystemName { get; set; }
}