using System.Collections.ObjectModel;

using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class UcdBlocksCollection : ObservableCollection<UcdBlockViewModel>
{
  public void Add(UcdBlock ub)
  {
    Add(new UcdBlockViewModel(ub));
  }

  public double MaxBlockNameWidth => this.Max(ub => ub.BlockName?.Length ?? 0)*12;
}