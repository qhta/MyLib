using System.Windows;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Helpers;

public class ViewModelsBindingProxy// : DependencyObject
{
  public ViewModelsBindingProxy() { }

  public _ViewModels Instance
  {
    get => _ViewModels.Instance;
  }

  //public static readonly DependencyProperty Instance =
  //  DependencyProperty.Register(nameof(Data), typeof(_ViewModels), typeof(ViewModelsBindingProxy), 
  //    new PropertyMetadata(null));
}