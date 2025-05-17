using System.Collections.ObjectModel;
using System.Diagnostics;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class UcdBlockViewModel : ViewModel<UcdBlock>, ILongTextViewModel
{
  /// <inheritdoc />
  public UcdBlockViewModel(UcdBlock model) : base(model)
  {
    // Initialize any additional properties or collections here if needed
  }

  public int? Id => Model.Id!;

  public CodeRange? Range
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

  public string? BlockName
  {
    get => Model.BlockName;
    set
    {
      if (Model.BlockName != null)
      {
        Model.BlockName = value;
        NotifyPropertyChanged(nameof(BlockName));
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

  public UcdRangeCollection Ranges
  {
    get
    {
      if (_Ranges == null)
      {
        _Ranges = new UcdRangeCollection();
        foreach (var range in Model.UcdRanges)
        {
          var vm = new UcdRangeViewModel(range);
          _Ranges.Add(vm);
        }
      }
      return _Ranges;
    }
  } 
  public UcdRangeCollection? _Ranges;

  public bool CanExpandLongText => !string.IsNullOrEmpty(Model.Comment);
  public string? LongText { get => Comment; set => Comment = value; }

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
}