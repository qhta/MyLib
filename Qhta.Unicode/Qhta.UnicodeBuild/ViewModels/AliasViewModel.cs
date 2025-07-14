using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for the Alias entity.
/// </summary>
/// <param name="model"></param>
public partial class AliasViewModel(Alias model) : ViewModel<Alias>(model)
{
  /// <summary>
  /// Ordinal number of the alias associated with the specific code point.
  /// </summary>
  public int Ord { get => Model.Ord; set { if (Model.Ord != value) { Model.Ord = value; NotifyPropertyChanged(nameof(Ord)); } } }
  /// <summary>
  /// Name of the alias, which is a short identifier for the alias.
  /// </summary>
  public string Name { get => Model.Name; set { if (Model.Name != value) { Model.Name = value; NotifyPropertyChanged(nameof(Name)); } } }
  /// <summary>
  /// Type of the alias, which is represented as an enumeration value.
  /// </summary>
  public AliasType Type { get => Model.Type; set { if (Model.Type != value) { Model.Type = value; NotifyPropertyChanged(nameof(Type)); } } }
}

