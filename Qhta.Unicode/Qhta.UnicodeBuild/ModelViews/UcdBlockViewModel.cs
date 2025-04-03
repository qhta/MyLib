using System.Diagnostics;

using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class UcdBlockViewModel: ViewModel<UcdBlock>, ILongTextViewModel
{
  /// <inheritdoc />
  public UcdBlockViewModel(UcdBlock model) : base(model)
  {
    // Initialize any additional properties or collections here if needed
  }

  public string? Range { get => Model.Range; set => Model.Range = value; }

  public string? BlockName { get => Model.BlockName; set => Model.BlockName = value; }

  public int? Start { get => Model.Start; set => Model.Start = value; }

  public int? End { get => Model.End; set => Model.End = value; }

  public string? Comment { get => Model.Comment; set => Model.Comment = value; }

  public WritingSystem? WritingSystem { get => Model.WritingSystem; set => Model.WritingSystem = value; }

  //public virtual ICollection<UcdRange> UcdRanges { get; set; } = new List<UcdRange>();

  //public UcdBlockViewModel(UcdBlock model, UcdBlocksCollection collection) : this(model)
  //{
  //  Collection = collection;
  //}

  //internal UcdBlocksCollection Collection { get; init; } = null!;

  //public double BlockNameWidth { get => Collection.GetMaxBlockNameWidth}

  public bool CanExpandRowHeight => !string.IsNullOrEmpty(Model.Comment) && Model.Comment.Length >50;
  
  private bool _isWrapped = false;
  public bool IsRowHeightExpanded
  {
    get => _isWrapped;
    set
    {
      if (_isWrapped != value)
      {
        _isWrapped = value;
        //Debug.WriteLine($"IsWrapped changed to {IsRowHeightExpanded}");
        NotifyPropertyChanged(nameof(IsRowHeightExpanded));
      }
    }
  }
}
