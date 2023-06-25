using System.Threading.Tasks;

namespace Qhta.MVVM
{
  /// <summary>
  /// Interface that defines properties and a method for loading object in background thread.
  /// </summary>
  public interface ILazyLoad
  {
    /// <summary>
    /// Determines whether a view model should load a model on init.
    /// </summary>
    bool LoadOnInit { get; set; }
    /// <summary>
    /// Determines whether a view model is loading a model.
    /// </summary>
    bool IsLoading { get; }
    /// <summary>
    /// Determines whether a view model has already loaded a model.
    /// </summary>
    bool IsLoaded { get; }
    /// <summary>
    /// A task to start loading.
    /// </summary>
    Task StartLoading();
  }
}
