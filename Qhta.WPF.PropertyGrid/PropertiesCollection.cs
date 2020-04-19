using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Qhta.TypeUtils;

namespace Qhta.WPF.PropertyGrid
{
  public class PropertiesCollection : ObservableCollection<IPropertyViewModel>
  {

    public virtual bool IsLoading
    {
      get => isLoading;
      set
      {
        if (isLoading != value)
        {
          isLoading = value;
          OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));
        }
      }
    }
    private bool isLoading;
  }
}
