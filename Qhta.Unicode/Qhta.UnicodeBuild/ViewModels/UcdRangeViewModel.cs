using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class UcdRangeViewModel: ViewModel<UcdRange>, ILongTextViewModel
{
  /// <inheritdoc />
  public UcdRangeViewModel(UcdRange model) : base(model)
  {
    // Initialize any additional properties or collections here if needed
  }

  public RangeModel? Range
  {
    get => Model.Range;
    set
    {
      if (Model.Range != value)
      {
        Model.Range = value;
        NotifyPropertyChanged(nameof(Range));
      }
    }
  }

  public string? RangeName
  {
    get => Model.RangeName;
    set
    {
      if (Model.RangeName != null)
      {
        Model.RangeName = value;
        NotifyPropertyChanged(nameof(RangeName));
      }
    }
  }

  public string? Comment
  {
    get => Model.Comment;
    set
    {
      if (Model.Comment != value)
      {
        Model.Comment = value;
        NotifyPropertyChanged(nameof(Comment));
      }
    }
  }

  public WritingSystem? WritingSystem { get => Model.WritingSystem; set => Model.WritingSystem = value; }
  public string? LongText { get => Comment; set => Comment=value; }
  public bool CanExpandRowHeight => false;//!string.IsNullOrEmpty(Model.Comment) && Model.Comment.Length >50;
  
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
