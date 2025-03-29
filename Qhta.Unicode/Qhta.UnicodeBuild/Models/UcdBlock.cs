using System;
using System.Collections.Generic;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.Unicode.Models;

public partial class UcdBlock
{
  public int? Id { get; set; }

  public string? Range { get; set; }

  public string? BlockName { get; set; }

  public int? Start { get; set; }

  public int? End { get; set; }

  public string? Comment { get; set; }

  public int? WritingSystemId { get; set; } // Foreign key property

  public WritingSystem? WritingSystem { get; set; } // Navigation property

  //public WritingSystemsCollection WritingSystems => _ViewModels.Instance.WritingSystems;

  //public virtual ICollection<UcdRange> UcdRanges { get; set; } = new List<UcdRange>();
}
