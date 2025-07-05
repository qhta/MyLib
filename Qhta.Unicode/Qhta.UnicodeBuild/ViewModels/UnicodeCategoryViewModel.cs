using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class UnicodeCategoryViewModel(UnicodeCategoryEntity model)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
  : ViewModel<UnicodeCategoryEntity>(model), IEquatable<UnicodeCategoryViewModel>
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
{
  public UnicodeCategoryViewModel() : this(new UnicodeCategoryEntity())
  {
    //Debug.WriteLine($"WritingSystemViewModel() {this}");
  }

  public UcdCategory Id => model.Id;

  public string Name => model.Ctg ?? string.Empty;

  public string? Description => model.Name;

  public bool Equals(UnicodeCategoryViewModel? other)
  {
    return other is not null && Id == other.Id;
  }
}