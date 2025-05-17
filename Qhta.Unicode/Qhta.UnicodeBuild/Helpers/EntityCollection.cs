using System.Collections.ObjectModel;
using System.ComponentModel;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Helpers;

public class EntityCollection<T>: ObservableCollection<T>
{

  public void NotifyPropertyChanged(string propertyName)
  {
    OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
  }

  public UcdBlockViewModel? SelectedItem
  {
    get => _SelectedItem;
    set
    {
      if (_SelectedItem != value)
      {
        _SelectedItem = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedItem)));
        NotifyPropertyChanged(nameof(SelectedItem));
      }
    }
  }

  private UcdBlockViewModel? _SelectedItem;
}