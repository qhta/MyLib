using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.Resources;

namespace Qhta.UnicodeBuild.ViewModels;

public class ScriptMappingViewModel(ScriptMapping scriptMapping) : ViewModel<ScriptMapping>(scriptMapping)
{
  public ScriptMappingViewModel() : this(new ScriptMapping())
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

  public WritingSystemViewModel? Script
  {
    get
    {
      var result = Model.Script is null ? null : _ViewModels.Instance.WritingSystems.FindByName(Model.Script);
      if (result is null && Model.Script is not null)
      {
        throw new InvalidOperationException(string.Format(Resources.Strings.WritingSystemNotFound, Model.Script));
      }
      return result;
    }
    set
    {
      if (value?.Name is not null)
      {
        if (value.Name != Model.Script)
        {
          Model.Script = value?.Name;
          NotifyPropertyChanged(nameof(Script));
        }
      }
      else
      {
        if (Model.Script is not null)
        {
          Model.Script = null;
          NotifyPropertyChanged(nameof(Script));
        }
      }
    }
  }
}