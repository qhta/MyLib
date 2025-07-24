using System.Diagnostics;
using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

/// <summary>
/// ViewModel for <see cref="WritingSystemMapping"/>.
/// </summary>
/// <param name="WritingSystemMapping"></param>
public class WritingSystemMappingViewModel(WritingSystemMapping WritingSystemMapping) : ViewModel<WritingSystemMapping>(WritingSystemMapping)
{
  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemMappingViewModel"/> class with a new <see cref="WritingSystemMapping"/> model.
  /// </summary>
  public WritingSystemMappingViewModel() : this(new WritingSystemMapping())
  {
  }

  /// <summary>
  /// Range of code points that this mapping applies to.
  /// </summary>
  public CodeRange Range
  {
    [DebuggerStepThrough] get => Model.Range!;
    set
    {
      if (Model.Range != value)
      {
        Model.Range = value!;
        NotifyPropertyChanged(nameof(Range));
      }
    }
  }

  /// <summary>
  /// Writing system associated with this mapping.
  /// </summary>
  public WritingSystemViewModel? WritingSystem
  {
    get
    {
      var result = Model.WritingSystemName is null ? null : _ViewModels.Instance.WritingSystems.FindByName(Model.WritingSystemName);
      if (result is null && Model.WritingSystemName is not null)
      {
        throw new InvalidOperationException(string.Format(Resources.Strings.WritingSystemNotFound, Model.WritingSystemName));
      }
      return result;
    }
    set
    {
      if (value?.Name is not null)
      {
        if (value.Name != Model.WritingSystemName)
        {
          Model.WritingSystemName = value?.Name;
          NotifyPropertyChanged(nameof(WritingSystem));
        }
      }
      else
      {
        if (Model.WritingSystemName is not null)
        {
          Model.WritingSystemName = null;
          NotifyPropertyChanged(nameof(WritingSystem));
        }
      }
    }
  }
}