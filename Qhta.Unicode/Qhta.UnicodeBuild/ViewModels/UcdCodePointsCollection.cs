using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public class UcdCodePointsCollection() : ObservableCollection<UcdCodePointViewModel>()
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

  private int _progressValue;
  public int ProgressValue
  {
    get => _progressValue;
    set
    {
      _progressValue = value;
      base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ProgressValue)));
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
}