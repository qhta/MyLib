using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for the NameGenMethodEntity.
/// </summary>
/// <param name="model"></param>
public class NameGenMethodViewModel(NameGenMethodEntity model)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
  : ViewModel<NameGenMethodEntity>(model), IEquatable<NameGenMethodViewModel>
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
  /// <summary>
  /// Initializes a new instance of the <see cref="NameGenMethodViewModel"/> class with a default model.
  /// </summary>
  public NameGenMethodViewModel() : this(new NameGenMethodEntity())
  {
  }

  /// <summary>
  /// Identifier for the writing system type.
  /// </summary>
  public NameGenMethod Id => model.Id;

  /// <summary>
  /// Name of the writing system type.
  /// </summary>
  public string Name => model.Name ?? string.Empty;

  /// <summary>
  /// Description of the writing system type.
  /// </summary>
  public string? Description => model.Description;

  /// <summary>
  /// Compares this instance with another <see cref="NameGenMethodViewModel"/> for equality based on the Id property.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(NameGenMethodViewModel? other)
  {
    return other is not null && Id == other.Id;
  }
}