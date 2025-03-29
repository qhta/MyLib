using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public partial class WritingSystemType
{
  [Key]
  public string Type { get; set; } = null!;

  public string? Description { get; set; }

  //public virtual ICollection<WritingSystem> WritingSystems { get; set; } = new List<WritingSystem>();
}
