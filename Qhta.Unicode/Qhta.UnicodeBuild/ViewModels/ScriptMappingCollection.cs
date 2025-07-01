using System.Windows;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.DeepCopy;

namespace Qhta.UnicodeBuild.ViewModels;

public class ScriptMappingCollection() : OrderedObservableCollection<ScriptMappingViewModel>((item) => item.Range)
{
  public ScriptMappingViewModel Add(ScriptMapping sm)
  {
    var sv = new ScriptMappingViewModel(sm);
    base.Add(sv);
    return sv;
  }

  public ScriptMappingViewModel? FindScript(int cp)
  {
    return this.FirstOrDefault(item => item.Range.Contains(cp));
  }
}