using System.Collections.ObjectModel;
using System.Diagnostics;

using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class UcdBlockViewModel : ViewModel<UcdBlock>, ILongTextViewModel, IRowHeightProvider, IComparable<UcdBlockViewModel>
{
  public UcdBlockViewModel() : base(new UcdBlock())
  {
    // Initialize any additional properties or collections here if needed
  }

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

  public string? Name
  {
    get => Model.BlockName;
    set
    {
      if (Model.BlockName != null)
      {
        Model.BlockName = value;
        NotifyPropertyChanged(nameof(Name));
      }
    }
  }

  public string? Description
  {
    get => Model.Comment;
    set
    {
      if (Model.Comment != value)
      {
        Model.Comment = value;
        NotifyPropertyChanged(nameof(Description));
      }
    }
  }

  public WritingSystemViewModel? WritingSystem
  {
    get
    {
      var result = Model.WritingSystemId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.WritingSystemId);
      return result;
    }
    set => Model.WritingSystemId = value?.Id;
  }

  public bool CanExpandLongText => !string.IsNullOrEmpty(Model.Comment);
  public string? LongText { get => Description; set => Description = value; }

  private bool _IsLongTextExpanded = false;
  public bool IsLongTextExpanded
  {
    get => _IsLongTextExpanded;
    set
    {
      if (_IsLongTextExpanded != value)
      {
        _IsLongTextExpanded = value;
        NotifyPropertyChanged(nameof(IsLongTextExpanded));
      }
    }
  }


  private double _RowHeight = 24;
  public double RowHeight
  {
    get => _RowHeight;
    set
    {
      if (_RowHeight != value)
      {
        _RowHeight = value;
        //Debug.WriteLine($"RowHeight changed to {RowHeight}");
        NotifyPropertyChanged(nameof(RowHeight));
      }
    }
  }


  public int CompareTo(UcdBlockViewModel? other)
  {
    if (other is null) return 1;
    if (Id == null && other.Id == null)
      return 0;
    if (Id == null)
      return -1;
    if (other.Id == null)
      return 1;
    return ((int)Id).CompareTo((int)(other.Id));
  }

}