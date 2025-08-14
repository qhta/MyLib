using System.Diagnostics;
using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.SF.Tools.Resources;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.Resources;
using Strings = Qhta.SF.Tools.Resources.Strings;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for a Unicode Character Database (UCD) block.
/// </summary>
public partial class UcdBlockViewModel : ViewModel<UcdBlock>, ILongTextViewModel, IComparable<UcdBlockViewModel>, 
  ISelectableItem,
  IRowHeightProvider, IErrorMessageProvider
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UcdBlockViewModel"/> class with a new <see cref="UcdBlock"/> model.
  /// </summary>
  public UcdBlockViewModel() : base(new UcdBlock())
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="UcdBlockViewModel"/> class with the specified <see cref="UcdBlock"/> model.
  /// </summary>
  /// <param name="model"></param>
  public UcdBlockViewModel(UcdBlock model) : base(model)
  {
  }

  /// <summary>
  /// Identifier of the block. This is the primary key in the database.
  /// </summary>
  public int? Id => Model.Id!;

  /// <summary>
  /// Code range of the block, represented as a string in the format "XXXX..YYYY".
  /// </summary>
  public CodeRange? Range
  {
    [DebuggerStepThrough] get => Model.Range;
    set
    {
      if (Model.Range != value)
      {
        Model.Range = value;
        NotifyPropertyChanged(nameof(Range));
      }
    }
  }

  /// <summary>
  /// Name of the block, which is a descriptive label for the block.
  /// </summary>
  public string? Name
  {
    [DebuggerStepThrough]
    get => Model.BlockName;
    set
    {
      if (Model.BlockName != value)
      {
        Model.BlockName = value;
        NotifyPropertyChanged(nameof(Name));
      }
    }
  }

  /// <summary>
  /// Description of the block, which provides additional context or information about its purpose.
  /// </summary>
  public string? Description
  {
    [DebuggerStepThrough] get => Model.Comment;
    set
    {
      if (Model.Comment != value)
      {
        Model.Comment = value;
        NotifyPropertyChanged(nameof(Description));
      }
    }
  }

  /// <summary>
  /// Main writing system associated with this UCD block, if applicable.
  /// </summary>
  public WritingSystemViewModel? WritingSystem
  {
    get
    {
      var result = Model.WritingSystemId is null ? null : _ViewModels.Instance.WritingSystems.FindById((int)Model.WritingSystemId);
      return result;
    }
    set => Model.WritingSystemId = value?.Id;
  }

  #region ILongTextViewModel implementation
  /// <summary>
  /// Gets a value indicating whether the long text can be expanded.
  /// </summary>
  public bool CanExpandLongText => !string.IsNullOrEmpty(Model.Comment);
  /// <summary>
  /// Long text description of the block, which can be expanded or collapsed.
  /// </summary>
  public string? LongText { get => Description; set => Description = value; }

  private bool _IsLongTextExpanded = false;
  /// <summary>
  /// Gets or sets a value indicating whether the long text is expanded.
  /// </summary>
  public bool IsLongTextExpanded
  {
    [DebuggerStepThrough] get => _IsLongTextExpanded;
    set
    {
      if (_IsLongTextExpanded != value)
      {
        _IsLongTextExpanded = value;
        NotifyPropertyChanged(nameof(IsLongTextExpanded));
      }
    }
  }
  #endregion

  /// <summary>
  /// Compares this instance with another <see cref="UcdBlockViewModel"/> instance.
  /// Used for sorting or ordering blocks based on their Ids.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
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

  #region IRowHeightProvider implementation
  /// <summary>
  /// Simple property to provide the height of the row in a UI.
  /// </summary>
  public double RowHeight
  {
    [DebuggerStepThrough] get => _RowHeight;
    set
    {
      if (_RowHeight != value)
      {
        _RowHeight = value;
        NotifyPropertyChanged(nameof(RowHeight));
      }
    }
  }
  private double _RowHeight = 24;
  #endregion

  #region IErrorMessageProvider implementation

  /// <summary>
  /// Gets or sets an error message associated with the view model.
  /// </summary>
  public string? ErrorMessage { [DebuggerStepThrough] get; set; }

  #endregion

  #region ISelectableItem implementation
  /// <summary>
  /// Name of the item, used for display purposes.
  /// </summary>
  public string DisplayName
  {
    get => Name ?? _DisplayName;
    set => _DisplayName = value;
  }

  private string _DisplayName = Strings.EmptyValue;

  /// <summary>
  /// Determines whether the item is selected in the UI.
  /// </summary>
  public bool IsSelected
  {
    get => _IsSelected;
    set
    {
      if (_IsSelected != value)
      {
        _IsSelected = value;
        NotifyPropertyChanged(nameof(IsSelected));
      }
    }
  }
  private bool _IsSelected = false;
  #endregion
}