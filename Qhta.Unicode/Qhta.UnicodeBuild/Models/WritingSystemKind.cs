using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public partial class WritingSystemKind
{
  [Key]
  public string Kind { get; set; } = null!;

  public string? Description { get; set; }

  //public virtual ICollection<WritingSystem> WritingSystems { get; set; } = new List<WritingSystem>();
}
