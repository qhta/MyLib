using System.Windows;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.DeepCopy;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemMappingCollection() : OrderedObservableCollection<WritingSystemMappingViewModel>((item) => item.Range)
{
  public WritingSystemMappingViewModel Add(WritingSystemMapping sm)
  {
    var sv = new WritingSystemMappingViewModel(sm);
    base.Add(sv);
    return sv;
  }

  public WritingSystemMappingViewModel? FindWritingSystem(int cp)
  {
    return this.FirstOrDefault(item => item.Range.Contains(cp));
  }
}