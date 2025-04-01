using System.Diagnostics;
using System.Windows.Input;

using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class WritingSystemViewModel: ViewModel<WritingSystem>
{
  public int? Id { get => Model.Id; set => Model.Id = value; }
  public string Name { get => Model.Name; set => Model.Name = value; }
  public string? Type { get => Model.Type; set => Model.Type = value; }
  public int? ParentId { get => Model.ParentId; set => Model.ParentId = value; }
  public string? Kind { get => Model.Kind; set => Model.Kind = value; }
  public bool? Starting { get => Model.Starting; set => Model.Starting = value; }
  public string? Keywords { get => Model.Keywords; set => Model.Keywords = value; }
  public string? Ctg { get => Model.Ctg; set => Model.Ctg = value; }
  public string? Iso { get => Model.Iso; set => Model.Iso = value; }
  public string? Abbr { get => Model.Abbr; set => Model.Abbr = value; }
  public string? Ext { get => Model.Ext; set => Model.Ext = value; }
  public string? Description { get => Model.Description; set => Model.Description = value; }
  public string? Aliases { get => Model.Aliases; set => Model.Aliases = value; }
  public virtual WritingSystemType? WritingSystemType { get => Model.WritingSystemType; set => Model.WritingSystemType = value; }
  public virtual WritingSystemKind? WritingSystemKind { get => Model.WritingSystemKind; set => Model.WritingSystemKind = value; }
  public virtual WritingSystem? Parent { get => Model.Parent; set => Model.Parent= value; }
  public virtual IEnumerable<WritingSystem>? SelectableWritingSystemParents => _ViewModels.Instance.SelectableWritingSystemParents;
  public virtual ICollection<WritingSystem>? Children => Model.Children;

  public int ChildrenCount => Children?.Count ?? 0;

  private bool _isExpanded;
  public bool IsExpanded
  {
    get => _isExpanded;
    set
    {
      if (_isExpanded != value)
      {
        _isExpanded = value;
        NotifyPropertyChanged(nameof(IsExpanded));
      }
    }
  }

  public ICommand ExpandCommand { get; set; }

  public WritingSystemViewModel(WritingSystem model) : base(model)
  {
    ExpandCommand = new RelayCommand(Expand);
  }

  private void Expand()
  {
    IsExpanded = !IsExpanded;
    Debug.WriteLine($"IsExpanded = {IsExpanded}");
  }

  ////public virtual ICollection<UcdBlock> UcdBlocks { get; set; }
}
