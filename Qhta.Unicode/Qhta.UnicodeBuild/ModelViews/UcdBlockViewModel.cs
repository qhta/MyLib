using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ModelViews;

namespace Qhta.Unicode.ViewModels;

public partial class UcdBlockViewModel(UcdBlock model) : ViewModel<UcdBlock>(model)
{
  public string? Range { get => Model.Range; set => Model.Range = value; }

  public string? BlockName { get => Model.BlockName; set => Model.BlockName = value; }

  //public int? Start { get; set; }

  //public int? End { get; set; }

  public string? Comment { get => Model.Comment; set => Model.Comment = value; }

  public WritingSystemViewModel? WritingSystem
  {
    get => _ViewModels.Instance.WritingSystems.FirstOrDefault(item=>item.Id==Model.WritingSystemId); 
    set => Model.WritingSystem = _ViewModels.Instance.WritingSystems.FirstOrDefault(item => i);
  }

  //public virtual ICollection<UcdRange> UcdRanges { get; set; } = new List<UcdRange>();
}
