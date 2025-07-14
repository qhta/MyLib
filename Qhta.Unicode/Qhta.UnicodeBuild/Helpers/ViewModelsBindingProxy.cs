using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Class that serves as a binding proxy for the ViewModels.
/// Used to bind the ViewModels static property Instance in the XAML resource file.
/// It is needed because the ViewModels class is static and cannot be used directly in XAML.
/// </summary>
public class ViewModelsBindingProxy// : DependencyObject
{
  /// <summary>
  /// Default constructor for the ViewModelsBindingProxy class.
  /// </summary>
  public ViewModelsBindingProxy() { }

  /// <summary>
  /// Instance property that provides access to the ViewModels singleton instance.
  /// </summary>
  public _ViewModels Instance => _ViewModels.Instance;
}