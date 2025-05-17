using System.Collections.ObjectModel;

using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class UcdRangeCollection : ObservableCollection<UcdRangeViewModel>
{


  public void Add(UcdRange ur)
  {
    Add(new UcdRangeViewModel(ur));
  }

  //public double MaxBlockNameWidth => this.Max(ub => ub.RangeName?.Length ?? 0)*12;
}