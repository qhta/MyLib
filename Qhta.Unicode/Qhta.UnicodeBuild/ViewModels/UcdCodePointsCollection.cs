using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public class UcdCodePointsCollection() : OrderedObservableCollection<UcdCodePointViewModel>((item) => item.Id)
{
  public UcdCodePointViewModel Add(UcdCodePoint ws)
  {
    var vm = new UcdCodePointViewModel(ws);
    base.Add(vm);
    IntDictionary[ws.Id] = vm;
    return vm;
  }

  private readonly Dictionary<int, UcdCodePointViewModel> IntDictionary = new Dictionary<int, UcdCodePointViewModel>();

  public UcdCodePointViewModel? FindById(int id)
  {
    return IntDictionary.GetValueOrDefault(id);
  }

  private bool _isBusy;
  public bool IsBusy
  {
    get => _isBusy;
    set
    {
      if (value != _isBusy)
      {
        _isBusy = value;
        base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsBusy)));
      }
    }
  }

  private int _progressValue;
  public int ProgressValue
  {
    get => _progressValue;
    set
    {
      if (value != _progressValue)
      {
        _progressValue = value;
        base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ProgressValue)));
      }
    }
  }

  private string? _statusMessage;
  public string? StatusMessage
  {
    get => _statusMessage;
    set
    {
      _statusMessage = value;
      base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(StatusMessage)));
    }
  }

  //public override int DataRecordsCount => _ViewModels.Instance.DBContext.CodePoints.Count();
}