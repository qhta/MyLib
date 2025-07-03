using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.Resources;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemMappingViewModel(WritingSystemMapping WritingSystemMapping) : ViewModel<WritingSystemMapping>(WritingSystemMapping)
{
  public WritingSystemMappingViewModel() : this(new WritingSystemMapping())
  {
    //Debug.WriteLine($"WritingSystemViewModel() {this}");
  }

  public CodeRange Range
  {
    get => Model.Range!;
    set
    {
      if (Model.Range != value)
      {
        Model.Range = value!;
        NotifyPropertyChanged(nameof(Range));
      }
    }
  }

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