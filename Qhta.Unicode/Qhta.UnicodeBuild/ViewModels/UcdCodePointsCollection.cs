using System.Collections.ObjectModel;
using System.Windows;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public class UcdCodePointsCollection() : ObservableCollection<UcdCodePointViewModel>()
{
  public UcdCodePointViewModel Add(UcdCodePoint ws)
  {
    var vm = new UcdCodePointViewModel(ws);
    _ViewModels.Instance.UcdCodePoints.Add(vm);
    return vm;
  }


}