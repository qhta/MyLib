using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public class ScriptMapping
{
  [Key]
  public string Range { get; set; } = null!;
  public string? Script { get; set; }
}