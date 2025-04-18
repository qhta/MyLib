using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public partial class WritingSystemKind
{
  [Key]
  public byte Id { get; set; }

  public string Kind { get; set; } = null!;

  public string? Description { get; set; }

}
