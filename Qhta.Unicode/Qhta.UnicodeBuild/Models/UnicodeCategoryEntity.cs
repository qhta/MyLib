using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qhta.Unicode.Models;

[Table("Categories")]
public partial class UnicodeCategoryEntity
{
  [Key]
  public UcdCategory Id { get; set; }

  public string Ctg { get; set; } = null!;

  public string? Name { get; set; }

  public string? Comment { get; set; }


}