using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qhta.Unicode.Models;

[Table("UcdCodePoints")]
public partial class UcdCodePoint
{
  [Column("Ord")]
  public int Id { get; set; }

  public string? Glyph { get; set; }

  public string Code { get; set; } = null!;

  public string? CharName { get; set; }

  public string? Description { get; set; }

  public string? Ctg { get; set; }

  public byte? Comb { get; set; }

  public string? Bidir { get; set; }

  public string? Decomposition { get; set; }

  public string? DecDigitVal { get; set; }

  public string? DigitVal { get; set; }

  public string? NumVal { get; set; }

  public string? Mirr { get; set; }

  public string? OldDescription { get; set; }

  public string? Comment { get; set; }

  public string? Upper { get; set; }

  public string? Lower { get; set; }

  public string? Title { get; set; }

  public int? Block { get; set; }

  public int? Area { get; set; }

  public int? Script { get; set; }

  public int? Language { get; set; }

  public int? Notation { get; set; }

  public int? SymbolSet { get; set; }

  public int? Subset { get; set; }

  public int? Artefact { get; set; }

  public virtual ICollection<Alias> Aliases { get; set; } = new List<Alias>();
}
