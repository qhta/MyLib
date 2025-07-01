using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemTypeViewModel(WritingSystemTypeEntity model)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
  : ViewModel<WritingSystemTypeEntity>(model), IEquatable<WritingSystemTypeViewModel>
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
  public WritingSystemTypeViewModel() : this(new WritingSystemTypeEntity())
  {
    //Debug.WriteLine($"WritingSystemViewModel() {this}");
  }

  public WritingSystemType Id => model.Id;

  public string Name => model.Type ?? string.Empty;

  public string? Description => model.Description;

  public bool Equals(WritingSystemTypeViewModel? other)
  {
    return other is not null && Id == other.Id;
  }
}