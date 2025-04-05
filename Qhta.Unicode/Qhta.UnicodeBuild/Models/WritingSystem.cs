using System;
using System.Collections.Generic;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.Unicode.Models;

public partial class WritingSystem
{
  public int? Id { get; set; }

  public string Name { get; set; } = null!;

  public string? Type { get; set; }

  public string? Kind { get; set; }

  public int? ParentId { get; set; }

  public bool? Starting { get; set; }

  public string? KeyPhrase { get; set; }

  public string? Ctg { get; set; }

  public string? Iso { get; set; }

  public string? Abbr { get; set; }

  public string? Ext { get; set; }

  public string? Description { get; set; }

  public string? Aliases { get; set; }

  public virtual WritingSystemKind? WritingSystemKind { get; set; }

  public virtual WritingSystemType? WritingSystemType { get; set; }

  public virtual WritingSystem? Parent { get; set; }

  public virtual ICollection<WritingSystem>? Children { get; set; }

  public int ChildrenCount => Children?.Count ?? 0;

  public virtual ICollection<UcdBlock>? UcdBlocks { get; set; }

}
