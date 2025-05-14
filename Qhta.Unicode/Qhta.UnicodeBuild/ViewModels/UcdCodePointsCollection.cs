using System.Windows;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public class UcdCodePointsCollection() : OrderedObservableCollection<UcdCodePointViewModel>((item) => item.CP)
{
  public UcdCodePointViewModel Add(UcdCodePoint ws)
  {
    var vm = _ViewModels.Instance.UcdCodePoints.FirstOrDefault(item => item.CP == ws.Id);
    if (vm == null)
    {
      vm = new UcdCodePointViewModel(ws);
      _ViewModels.Instance.UcdCodePoints.Add(vm);
    }
    return vm;
  }


}