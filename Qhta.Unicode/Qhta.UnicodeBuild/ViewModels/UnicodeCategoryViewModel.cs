using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Resources;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for representing a Unicode category.
/// </summary>
/// <param name="model"></param>
public class UnicodeCategoryViewModel(UnicodeCategoryEntity model)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
  : ViewModel<UnicodeCategoryEntity>(model), IEquatable<UnicodeCategoryViewModel>, IComparable<UnicodeCategoryViewModel>, ISelectableItem
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UnicodeCategoryViewModel"/> class with a default model.
  /// </summary>
  public UnicodeCategoryViewModel() : this(new UnicodeCategoryEntity())
  {
  }

  /// <summary>
  /// Identifier for the Unicode category.
  /// </summary>
  public UcdCategory Id => model.Id;

  /// <summary>
  /// Name of the Unicode category, typically used for display purposes.
  /// </summary>
  public string? Name => model.Name;

  /// <summary>
  /// Description of the Unicode category, providing additional context or information.
  /// </summary>
  public string? Description => model.Name;

  /// <summary>
  /// Compare the current instance with another UnicodeCategoryViewModel instance for equality.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(UnicodeCategoryViewModel? other)
  {
    return other is not null && Id == other.Id;
  }

  /// <summary>
  /// Compares the current instance with another UnicodeCategoryViewModel instance.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public int CompareTo(UnicodeCategoryViewModel? other)
  {
    if (other is null) return 1;
     return Id.CompareTo(other.Id);
  }

  #region ISelectableItem implementation
  /// <summary>
  /// Name of the item, used for display purposes.
  /// </summary>
  public string DisplayName
  {
    get => Name ?? _DisplayName;
    set => _DisplayName = value;
  }

  private string _DisplayName = DataStrings.EmptyValue;

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