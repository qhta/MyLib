using System.Collections.ObjectModel;

using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class UcdBlocksCollection : ObservableCollection<UcdBlockViewModel>
{
  public void Add(UcdBlock ub)
  {
    var vm = new UcdBlockViewModel(ub);
    Add(vm);
  }

  public double MaxBlockNameWidth => this.Max(ub => ub.BlockName?.Length ?? 0)*12;
}