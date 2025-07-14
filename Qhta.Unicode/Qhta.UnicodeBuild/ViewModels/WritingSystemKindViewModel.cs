using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for representing a writing system kind.
/// </summary>
/// <param name="model"></param>
public class WritingSystemKindViewModel(WritingSystemKindEntity model)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
  : ViewModel<WritingSystemKindEntity>(model), IEquatable<WritingSystemKindViewModel>
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemKindViewModel"/> class using a default <see
  /// cref="WritingSystemKindEntity"/>.
  /// </summary>
  public WritingSystemKindViewModel() : this(new WritingSystemKindEntity())
  {
  }

  /// <summary>
  /// Identifier for the writing system kind, represented as a <see cref="WritingSystemKind"/> enum.
  /// </summary>
  public WritingSystemKind Id => model.Id;

  /// <summary>
  /// Name of the writing system kind.
  /// </summary>
  public string Name => model.Name ?? string.Empty;

  /// <summary>
  /// Description of the writing system kind.
  /// </summary>
  public string? Description => model.Description;

  /// <summary>
  /// Compares this instance with another <see cref="WritingSystemKindViewModel"/> instance for equality.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(WritingSystemKindViewModel? other)
  {
    return other is not null && Id == other.Id;
  }
}