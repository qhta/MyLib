using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;


namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for a writing system.
/// </summary>
/// <param name="model"></param>
public class WritingSystemViewModel(WritingSystem model)
  : EntityViewModel<WritingSystem>(model), ILongTextViewModel, IEquatable<WritingSystemViewModel>, IComparable<WritingSystemViewModel>,// INotifyDataErrorInfo
  IRowHeightProvider, IErrorMessageProvider
{
  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemViewModel"/> class with the specified model.
  /// </summary>
  public WritingSystemViewModel() : this(new WritingSystem())
  {
  }

  /// <summary>
  /// Collection that this writing system belongs to.
  /// </summary>
  public WritingSystemsCollection? Collection { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Gets or sets the identifier for the model.
  /// </summary>
  public int? Id
  {
    [DebuggerStepThrough]
    get => Model.Id;
    set => ChangeModelProperty(nameof(Id), value);
  }

  /// <summary>
  /// Gets or sets the name of the writing system.
  /// It is required property and must not be null or empty.
  /// </summary>
  [Required]
  public string? Name
  {
    [DebuggerStepThrough]
    get => Model.Name;
    set => ChangeModelProperty(nameof(Name), value);

  }

  /// <summary>
  /// Full name of the writing system, combining the name and type in a readable format.
  /// It is used for display purposes in the UI.
  /// </summary>
  public string FullName => Model.Name + " " + Type.ToString()?.ToLower();


  /// <summary>
  /// Type of the writing system, such as script, language, or notation.
  /// It is a required property and must not be null.
  /// </summary>
  [Required]
  public WritingSystemType? Type
  {
    [DebuggerStepThrough]
    get => Model.Type;
    set => ChangeModelProperty(nameof(Type), value);
  }

  /// <summary>
  /// Gets or sets the kind of writing system represented by this instance.
  /// </summary>
  public WritingSystemKind? Kind
  {
    [DebuggerStepThrough]
    get => Model.Kind;
    set => ChangeModelProperty(nameof(Kind), value);
  }

  /// <summary>
  /// Identifier of the parent writing system, if this writing system is a child of another.
  /// </summary>
  public int? ParentId
  {
    [DebuggerStepThrough]
    get => Model.ParentId;
    set => ChangeModelProperty(nameof(ParentId), value);
  }

  /// <summary>
  /// Key phrase which identifies the writing system in CodePoint Description field.
  /// </summary>
  public string? KeyPhrase
  {
    [DebuggerStepThrough]
    get => Model.KeyPhrase;
    set => ChangeModelProperty(nameof(KeyPhrase), value);
  }

  /// <summary>
  /// Code for the Unicode Category (Ctg) of the writing system.
  /// </summary>
  public string? Ctg
  {
    [DebuggerStepThrough]
    get => Model.Ctg;
    set => ChangeModelProperty(nameof(Ctg), value);
  }

  /// <summary>
  /// ISO code for the writing system, if applicable.
  /// </summary>
  public string? Iso
  {
    [DebuggerStepThrough]
    get => Model.Iso;
    set => ChangeModelProperty(nameof(Iso), value);
  }

  /// <summary>
  /// Abbreviation for the writing system used in the target serialization method.
  /// </summary>
  public string? Abbr
  {
    [DebuggerStepThrough]
    get => Model.Abbr;
    set => ChangeModelProperty(nameof(Abbr), value);
  }

  /// <summary>
  /// Extension string added to the Abbr field to provide unique identification for the writing system.
  /// </summary>
  public string? Ext
  {
    [DebuggerStepThrough]
    get => Model.Ext;
    set => ChangeModelProperty(nameof(Ext), value);
  }

  /// <summary>
  /// Description of the writing system, providing additional context or information.
  /// </summary>
  [DataType(DataType.MultilineText)]
  public string? Description
  {
    [DebuggerStepThrough]
    get => Model.Description;
    set => ChangeModelProperty(nameof(Description), value);
  }

  /// <summary>
  /// Exposed property for the parent writing system as a ViewModel.
  /// </summary>
  public virtual WritingSystemViewModel? Parent
  {
    [DebuggerStepThrough]
    get => _ViewModels.Instance.WritingSystems.FirstOrDefault(vm => vm.Id == Model.ParentId);
    set
    {
      if (value == Parent)
        return; // No change
      var parentId = value?.Id;
      if (parentId == 0)
        parentId = null;

      if (Parent != null)
      {
        Parent.Children?.Remove(this);
      }
      Model.ParentId = parentId;
      NotifyPropertyChanged(nameof(Parent));
      var parent = Parent;
      if (parent != null)
      {
        if (parent.Children == null)
          parent.GetOrCreateChildrenList();
        if (!parent.Children!.Contains(this))
        {
          // Add this writing system to the parent's children collection

          parent.Children.Add(this);
          parent.NotifyPropertyChanged(nameof(Children));
        }
      }
      NotifyPropertyChanged(nameof(ParentId));
    }
  }

  /// <summary>
  /// Provides information about whether this writing system is used in any UCD blocks or has children.
  /// </summary>
  public virtual bool IsUsed => Model.UcdBlocks?.Count > 0 || Model.Children?.Count > 0;

  /// <summary>
  /// Collection of child writing systems associated with this writing system.
  /// </summary>
  public virtual WritingSystemsCollection? Children
  {
    get
    {
      if (_Children == null)
      {
        if (Model.Children == null)
          return null;
        _Children = new WritingSystemsCollection(this, Model.Children.OrderBy(ws => ws.Name).ToList());
      }
      return _Children;
    }
  }
  private WritingSystemsCollection? _Children;

  /// <summary>
  /// Gets a collection of child WritingSystems. If it does not exist, it is created even if Model Children collection is null;
  /// </summary>
  /// <returns></returns>
  public WritingSystemsCollection? GetOrCreateChildrenList()
  {
    if (_Children == null)
      _Children = new WritingSystemsCollection(this);
    return _Children;
  }

  /// <summary>
  /// Indicates whether the current item is expanded.
  /// </summary>
  public bool IsExpanded
  {
    [DebuggerStepThrough]
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
  private bool _isExpanded;

  #region ILongTextViewModel implementation

  /// <summary>
  /// Long text property is the Description property of the writing system.
  /// </summary>
  public string? LongText { get => Description; set => Description = value; }

  /// <summary>
  /// Can the long text be expanded in the UI?
  /// </summary>
  public bool CanExpandLongText
  {
    get
    {
      var longText = LongText;
      if (longText == null)
        return false;
      var collectionLongTextColumnWidth = Collection?.LongTextColumnWidth ?? 300;
      var maxRowHeight = LongTextColumn.EvaluateTextHeight(longText, collectionLongTextColumnWidth);
      var actualRowHeight = RowHeight;
      if (double.IsNaN(actualRowHeight))
        actualRowHeight = 26;
      return maxRowHeight > actualRowHeight;
    }
  }



  /// <summary>
  /// Indicates whether the long text is expanded in the UI.
  /// </summary>
  public bool IsLongTextExpanded
  {
    [DebuggerStepThrough]
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
  private bool _IsLongTextExpanded;
  #endregion

  private readonly Dictionary<string, ValidationResultEx> errors = new();

  //#region Validation functionality
  ///// <summary>
  ///// Validates the specified property of the writing system.
  ///// </summary>
  ///// <param name="propertyName"></param>
  ///// <returns></returns>
  //public bool Validate(string propertyName)
  //{
  //  bool ok = true;
  //  if (propertyName == nameof(Name))
  //  {
  //    errors.Remove(propertyName);
  //    if (string.IsNullOrEmpty(Name))
  //      errors.Add(propertyName, new ValidationResultEx("Name cannot be empty", Severity.Error));
  //    else
  //    if (!Regex.IsMatch(Name, "^[a-zA-Z]+( [a-zA-Z0-9]+)*$"))
  //      errors.Add(propertyName, new ValidationResultEx("Invalid name string", Severity.Error));
  //    ok = false;
  //  }
  //  else
  //  {
  //    errors.Remove(propertyName);
  //    ok = true;
  //  }

  //  ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
  //  return ok;
  //}

  //IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
  //{
  //  if (propertyName != null && errors.TryGetValue(propertyName, out var error))
  //  {
  //    yield return error;
  //  }
  //  yield return null;
  //}

  ///// <summary>
  ///// Checks if there are any validation errors in the writing system.
  ///// </summary>
  //public bool HasErrors => errors.Count > 0;

  ///// <summary>
  ///// Adds an error message for the specified property.
  ///// </summary>
  ///// <param name="propertyName"></param>
  ///// <param name="message"></param>
  ///// <param name="severity"></param>
  //public void AddError(string propertyName, string message, Severity severity = Severity.Error)
  //{
  //  errors.Remove(propertyName);
  //  errors.Add(propertyName, new ValidationResultEx(message, severity));
  //  ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

  //}
  ///// <summary>
  ///// Event that is raised when the validation errors for a property change.
  ///// </summary>
  //public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

  //#endregion


  /// <summary>
  /// Compares this instance with another <see cref="WritingSystemViewModel"/> instance for equality.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(WritingSystemViewModel? other)
  {
    if (other == null)
      return false;
    if (this.Name == null || other.Name == null)
      return true;
    return Id == other.Id;
  }

  /// <summary>
  /// Compares this instance with another <see cref="WritingSystemViewModel"/> instance using the Id property.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public int CompareTo(WritingSystemViewModel? other)
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

  /// <summary>
  /// Converts the writing system to a string representation.
  /// </summary>
  /// <returns></returns>
  public override String ToString()
  {
    if (Name == null)
      return "<null>";
    if (Name == "")
      return "<empty>";
    return Name + " " + Type?.ToString()?.ToLower();
  }

  #region IRowHeightProvider implementation
  /// <summary>
  /// Simple property to provide the height of the row in a UI.
  /// </summary>
  public double RowHeight
  {
    [DebuggerStepThrough]
    get => _RowHeight;
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
}
