using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
  : ViewModel<WritingSystem>(model), ILongTextViewModel, IEquatable<WritingSystemViewModel>, IComparable<WritingSystemViewModel>,// INotifyDataErrorInfo
  IRowHeightProvider
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
  public WritingSystemsCollection? Collection { get; set; }

  /// <summary>
  /// Gets or sets the identifier for the model.
  /// </summary>
  public int? Id
  {
    get => Model.Id;
    set
    {
      if (Model.Id != value)
      {
        Model.Id = value;
        NotifyPropertyChanged(nameof(Id));
      }
    }
  }

  /// <summary>
  /// Gets or sets the name of the writing system.
  /// It is required property and must not be null or empty.
  /// </summary>
  [Required]
  public string? Name
  {
    get => Model.Name;
    set
    {
      if (Model.Name != value)
      {
        var oldValue = Model.Name;
        Model.Name = value;
        NotifyPropertyChanged(nameof(Name), oldValue, value);
      }
    }
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
    get => Model.Type;
    set
    {
      if (Model.Type != value)
      {
        Model.Type = value;
        NotifyPropertyChanged(nameof(Type));
      }
    }
  }

  /// <summary>
  /// Gets or sets the kind of writing system represented by this instance.
  /// </summary>
  public WritingSystemKind? Kind
  {
    get => Model.Kind;
    set
    {
      if (Model.Kind != value)
      {
        Model.Kind = value;
        NotifyPropertyChanged(nameof(Kind));
      }
    }
  }

  /// <summary>
  /// Identifier of the parent writing system, if this writing system is a child of another.
  /// </summary>
  public int? ParentId
  {
    get => Model.ParentId;
    set
    {
      if (Model.ParentId != value)
      {
        Model.ParentId = value;
        NotifyPropertyChanged(nameof(ParentId));
      }
    }
  }

  /// <summary>
  /// Key phrase which identifies the writing system in CodePoint Description field.
  /// </summary>
  public string? KeyPhrase
  {
    get => Model.KeyPhrase;
    set
    {
      if (Model.KeyPhrase != value)
      {
        Model.KeyPhrase = value;
        NotifyPropertyChanged(nameof(KeyPhrase));
      }
    }
  }

  /// <summary>
  /// Code for the Unicode Category (Ctg) of the writing system.
  /// </summary>
  public string? Ctg
  {
    get => Model.Ctg;
    set
    {
      if (Model.Ctg != value)
      {
        Model.Ctg = value;
        NotifyPropertyChanged(nameof(Ctg));
      }
    }
  }

  /// <summary>
  /// ISO code for the writing system, if applicable.
  /// </summary>
  public string? Iso
  {
    get => Model.Iso;
    set
    {
      if (Model.Iso != value)
      {
        Model.Iso = value;
        NotifyPropertyChanged(nameof(Iso));
      }
    }
  }

  /// <summary>
  /// Abbreviation for the writing system used in the target serialization method.
  /// </summary>
  public string? Abbr
  {
    get => Model.Abbr;
    set
    {
      if (Model.Abbr != value)
      {
        Model.Abbr = value;
        NotifyPropertyChanged(nameof(Abbr));
      }
    }
  }

  /// <summary>
  /// Extension string added to the Abbr field to provide unique identification for the writing system.
  /// </summary>
  public string? Ext
  {
    get => Model.Ext;
    set
    {
      if (Model.Ext != value)
      {
        Model.Ext = value;
        NotifyPropertyChanged(nameof(Ext));
      }
    }
  }

  /// <summary>
  /// Description of the writing system, providing additional context or information.
  /// </summary>
  [DataType(DataType.MultilineText)]
  public string? Description
  {
    get => Model.Description;
    set
    {
      if (Model.Description != value)
      {
        Model.Description = value;
        NotifyPropertyChanged(nameof(Description));
      }
    }
  }

  /// <summary>
  /// Exposed property for the parent writing system as a ViewModel.
  /// </summary>
  public virtual WritingSystemViewModel? Parent
  {
    get => _ViewModels.Instance.WritingSystems.FirstOrDefault(vm => vm.Id == Model.ParentId);
    set
    {
      var parentId = value?.Id;
      if (parentId == 0)
        parentId = null;

      if (Parent != null)
      {
        Parent.Children?.Remove(this);
      }
      Model.ParentId = parentId;
      NotifyPropertyChanged(nameof(Parent));
      if (Parent?.Children != null)
      {
        Parent.Children?.Add(this);
        Parent.NotifyPropertyChanged(nameof(Children));
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
  /// Indicates whether the current item is expanded.
  /// </summary>
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
    if (Id==null && other.Id == null)
      return 0;
    if (Id == null)
      return -1;
    if (other.Id == null)
      return 1;
    return  ((int)Id).CompareTo((int)(other.Id));
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

  /// <summary>
  /// Stores the height of a row in the UI.
  /// </summary>
  public double RowHeight { get; set; } = Double.NaN;

  /// <summary>
  /// Stores the height of a row in the UI.
  /// </summary>
  public double MaxRowHeight { get; set; } = Double.NaN;
}
