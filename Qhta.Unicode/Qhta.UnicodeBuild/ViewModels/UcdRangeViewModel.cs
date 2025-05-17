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

  public CodeRange? Range
  {
    get => Model.Range;
    set
    {
      if (Model.Range != value)
      {
        Model.Range = value;
        NotifyPropertyChanged(nameof(Range));
        _UcdBlock = null; // force recalculation of UcdBlock
        NotifyPropertyChanged(nameof(UcdBlock));
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

  public WritingSystemViewModel? WritingSystem
  {
    get
    {
      var result = Model.WritingSystemId is null ? null : _ViewModels.Instance.WritingSystemViewModels[(int)Model.WritingSystemId];
      return result;
    }
    set => Model.WritingSystemId = value?.Id;
  }

  public string? LongText { get => Comment; set => Comment=value; }
  public bool CanExpandLongText => !string.IsNullOrEmpty(Model.Comment);
  
  private bool _IsLongTextExpanded = false;
  public bool IsLongTextExpanded
  {
    get => _IsLongTextExpanded;
    set
    {
      if (_IsLongTextExpanded != value)
      {
        _IsLongTextExpanded = value;
        //Debug.WriteLine($"IsWrapped changed to {IsRowHeightExpanded}");
        NotifyPropertyChanged(nameof(IsLongTextExpanded));
      }
    }
  }

  public bool Contains(int code)
  {
    if (Range is null)
      return false;
    if (Range.End is null)
      return Range.Start == code;
    return Range.Start <= code && Range.End >= code;
  }

  public UcdBlockViewModel? UcdBlock
  {
    get
    {
      if (_UcdBlock is not null)
        return _UcdBlock;
      var code = Range?.Start;
      if (code is null)
        return null!;
      _UcdBlock = _ViewModels.Instance.UcdBlocks.FirstOrDefault(x => x.Range != null && x.Range.Start <= code && x.Range.End >= code);
      return _UcdBlock;
    }
  }

  private UcdBlockViewModel? _UcdBlock;
}
