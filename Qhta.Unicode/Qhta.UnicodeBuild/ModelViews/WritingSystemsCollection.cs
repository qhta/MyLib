using System.Collections.ObjectModel;

using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemsCollection : ObservableCollection<WritingSystemViewModel>
{
  public WritingSystemsCollection()
  {
  }

  public WritingSystemsCollection(IEnumerable<WritingSystem> ws)
  {
    foreach (var w in ws)
    {
      var vm = _ViewModels.Instance.AllWritingSystems.FirstOrDefault(item => item.Id == w.Id);
      if (vm == null)
      {
        vm = new WritingSystemViewModel(w);
        if (this != _ViewModels.Instance.AllWritingSystems) _ViewModels.Instance.AllWritingSystems.Add(vm);
      }
      Add(vm);
    }
  }

  public void Add(WritingSystem ws)
  {
    Add(new WritingSystemViewModel(ws));
  }

}