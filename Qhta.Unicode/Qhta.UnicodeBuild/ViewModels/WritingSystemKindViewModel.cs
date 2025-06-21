using System.ComponentModel;

using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemKindViewModel(WritingSystemKindEntity model)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
  : ViewModel<WritingSystemKindEntity>(model), IEquatable<WritingSystemKindViewModel>
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
  public WritingSystemKindViewModel() : this(new WritingSystemKindEntity())
  {
    //Debug.WriteLine($"WritingSystemViewModel() {this}");
  }

  public WritingSystemKind Id => model.Id;

  public string Name => model.Kind ?? string.Empty;

  public string? Description => model.Description;

  public bool Equals(WritingSystemKindViewModel? other)
  {
    return other is not null && Id == other.Id;
  }
}