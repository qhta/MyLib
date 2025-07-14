using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for the WritingSystemTypeEntity.
/// </summary>
/// <param name="model"></param>
public class WritingSystemTypeViewModel(WritingSystemTypeEntity model)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
  : ViewModel<WritingSystemTypeEntity>(model), IEquatable<WritingSystemTypeViewModel>
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemTypeViewModel"/> class with a default model.
  /// </summary>
  public WritingSystemTypeViewModel() : this(new WritingSystemTypeEntity())
  {
  }

  /// <summary>
  /// Identifier for the writing system type.
  /// </summary>
  public WritingSystemType Id => model.Id;

  /// <summary>
  /// Name of the writing system type.
  /// </summary>
  public string Name => model.Name ?? string.Empty;

  /// <summary>
  /// Description of the writing system type.
  /// </summary>
  public string? Description => model.Description;

  /// <summary>
  /// Compares this instance with another <see cref="WritingSystemTypeViewModel"/> for equality based on the Id property.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(WritingSystemTypeViewModel? other)
  {
    return other is not null && Id == other.Id;
  }
}