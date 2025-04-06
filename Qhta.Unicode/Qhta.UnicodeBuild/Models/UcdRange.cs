using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qhta.Unicode.Models;

public partial class UcdRange
{
  [Key]
  public string Range { get; set; } = null!;

  //public int? BlockId { get; set; }

  //public string? RangeName { get; set; }

  //public string? Standard { get; set; }

  //public int? WritingSystemId { get; set; }

  //public string? Language { get; set; }

  //public string? Comment { get; set; }

  //public WritingSystem? WritingSystem { get; set; } // Navigation property

  //public virtual UcdBlock? BlockRangeNavigation { get; set; }
}
