using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.Unicode.ViewModels;

public partial class WritingSystemViewModel(WritingSystem model): ViewModel<WritingSystem>(model)
{
  public int? Id { get => Model.Id; set => Model.Id = value; }
  public string Name { get => Model.Name; set => Model.Name = value; }
  public string? Type { get => Model.Type; set => Model.Type = value; }
  //public int? ParentId { get => Model.ParentId; set => Model.ParentId = value; }
  public string? Kind { get => Model.Kind; set => Model.Kind = value; }
  public bool? Starting { get => Model.Starting; set => Model.Starting = value; }
  public string? Keywords { get => Model.Keywords; set => Model.Keywords = value; }
  public string? Ctg { get => Model.Ctg; set => Model.Ctg = value; }
  public string? Iso { get => Model.Iso; set => Model.Iso = value; }
  public string? Abbr { get => Model.Abbr; set => Model.Abbr = value; }
  public string? Ext { get => Model.Ext; set => Model.Ext = value; }
  public string? Description { get => Model.Description; set => Model.Description = value; }
  public string? Aliases { get => Model.Aliases; set => Model.Aliases = value; }
  //public virtual ICollection<WritingSystem> InverseParent { get; set; } = new List<WritingSystem>();
  //public virtual WritingSystemKind? KindNavigation { get; set; }
  ////public virtual WritingSystem? Parent { get; set; }
  //public virtual WritingSystemType? TypeNavigation { get; set; }
  //public virtual ICollection<UcdBlock> UcdBlocks { get; set; }
}
